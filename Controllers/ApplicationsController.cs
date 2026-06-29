using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Data;
using PawdoptApp.Models;
using PawdoptApp.Services;

namespace PawdoptApp.Controllers;

[Authorize]
public class ApplicationsController : Controller
{
    private readonly ApplicationDbContext         _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationService           _appService;

    public ApplicationsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ApplicationService appService)
    {
        _context     = context;
        _userManager = userManager;
        _appService  = appService;
    }

    // ── GET /Applications ────────────────────────────────────────────────
    // Adopter   → own applications only
    // Rehomer   → applications for their pet listings only
    // Admin     → all applications
    public async Task<IActionResult> Index(string? status, string? sort)
    {
        var userId = _userManager.GetUserId(User);
        var user   = await _userManager.GetUserAsync(User);

        IQueryable<AdoptionApplication> query = _context.AdoptionApplications
            .Include(a => a.Adopter)
            .Include(a => a.PetListing);

        if (User.IsInRole("Admin"))
        {
            // sees all — no filter
        }
        else if (User.IsInRole("Rehomer"))
        {
            var listingIds = await _context.PetListings
                .Where(l => l.RehomerId == userId)
                .Select(l => l.Id)
                .ToListAsync();
            query = query.Where(a => listingIds.Contains(a.PetListingId));
        }
        else
        {
            query = query.Where(a => a.AdopterId == userId);
        }

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(a => a.Status == status);

        query = sort switch
        {
            "oldest" => query.OrderBy(a => a.SubmittedAt),
            "status" => query.OrderBy(a => a.Status).ThenByDescending(a => a.SubmittedAt),
            _        => query.OrderByDescending(a => a.SubmittedAt)
        };

        var apps = await query.ToListAsync();

        ViewData["StatusFilter"] = status ?? "";
        ViewData["Sort"]         = sort ?? "newest";
        ViewData["UserRole"]     = user?.UserType ?? "Adopter";
        return View(apps);
    }

    // ── GET /Applications/Details/5 ──────────────────────────────────────
    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);

        var app = await _context.AdoptionApplications
            .Include(a => a.Adopter)
            .Include(a => a.PetListing)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return NotFound();

        if (!await CanAccessAsync(app, userId))
            return Forbid();

        ViewData["UserRole"] = (await _userManager.GetUserAsync(User))?.UserType ?? "Adopter";
        return View(app);
    }

    // ── GET /Applications/UpdateStatus/5 ────────────────────────────────
    [Authorize(Roles = "Rehomer,Admin")]
    public async Task<IActionResult> UpdateStatus(int id)
    {
        var userId = _userManager.GetUserId(User);

        var app = await _context.AdoptionApplications
            .Include(a => a.Adopter)
            .Include(a => a.PetListing)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return NotFound();

        if (!User.IsInRole("Admin") && !await RehomerOwnsAppAsync(app, userId))
            return Forbid();

        return View(app);
    }

    // ── POST /Applications/UpdateStatus/5 ───────────────────────────────
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Rehomer,Admin")]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus, string? reviewNotes)
    {
        var userId = _userManager.GetUserId(User);

        var app = await _context.AdoptionApplications
            .Include(a => a.Adopter)
            .Include(a => a.PetListing)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return NotFound();

        if (!User.IsInRole("Admin") && !await RehomerOwnsAppAsync(app, userId))
            return Forbid();

        var (success, error) = await _appService.ApplyStatusChangeAsync(app, newStatus, reviewNotes);

        if (!success)
        {
            TempData["AppError"] = error;
            return RedirectToAction(nameof(UpdateStatus), new { id });
        }

        TempData["AppSuccess"] = $"Application marked as {newStatus.Label()}.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // ── POST /Applications/Withdraw/5 (Adopter) ─────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(int id)
    {
        var userId = _userManager.GetUserId(User);

        var app = await _context.AdoptionApplications
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return NotFound();

        if (!User.IsInRole("Admin") && app.AdopterId != userId)
            return Forbid();

        if (app.Status != "Pending" && app.Status != "UnderReview")
        {
            TempData["AppError"] = "Only pending or under-review applications can be withdrawn.";
            return RedirectToAction(nameof(Details), new { id });
        }

        app.Status = nameof(ApplicationStatus.Withdrawn);
        await _context.SaveChangesAsync();

        TempData["AppSuccess"] = "Application withdrawn successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ──────────────────────────────────────────────────────────
    private async Task<bool> CanAccessAsync(AdoptionApplication app, string? userId)
    {
        if (User.IsInRole("Admin")) return true;
        if (User.IsInRole("Rehomer")) return await RehomerOwnsAppAsync(app, userId);
        return app.AdopterId == userId;
    }

    private async Task<bool> RehomerOwnsAppAsync(AdoptionApplication app, string? userId)
    {
        var listing = await _context.PetListings
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == app.PetListingId);
        return listing?.RehomerId == userId;
    }
}
