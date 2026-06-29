using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Data;
using PawdoptApp.Models;
using PawdoptApp.Services;

namespace PawdoptApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext         _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationService           _appService;
    private readonly IWebHostEnvironment          _env;

    public AdminController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ApplicationService appService,
        IWebHostEnvironment env)
    {
        _context     = context;
        _userManager = userManager;
        _appService  = appService;
        _env         = env;
    }

    // ── GET /Admin/ListingDetail/5 — full review before approve/reject ────
    public async Task<IActionResult> ListingDetail(int id)
    {
        var listing = await _context.PetListings
            .Include(l => l.Rehomer)
            .FirstOrDefaultAsync(l => l.Id == id);
        if (listing == null) return NotFound();

        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "listings", id.ToString());
        ViewData["Photos"] = Directory.Exists(uploadDir)
            ? Directory.GetFiles(uploadDir).Select(f => $"/uploads/listings/{id}/{Path.GetFileName(f)}").ToList()
            : new List<string>();

        return View(listing);
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        var listings = await _context.PetListings
            .Include(l => l.Rehomer)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        ViewData["Users"]         = users;
        ViewData["Listings"]      = listings;
        ViewData["CurrentUserId"] = _userManager.GetUserId(User);
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveListing(int id, string? adminNote)
    {
        var listing = await _context.PetListings
            .Include(l => l.Rehomer)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (listing == null) return NotFound();

        listing.Status     = "Approved";
        listing.ApprovedAt = DateTime.UtcNow;
        listing.AdminNote  = adminNote;

        _context.Notifications.Add(new Notification
        {
            UserId    = listing.RehomerId,
            Type      = "listing_approved",
            Title     = $"Your listing for {listing.Name} was approved!",
            Body      = "Your pet listing is now live and visible to adopters.",
            LinkUrl   = "/Rehome/List",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        TempData["AdminMsg"] = $"{listing.Name}'s listing approved.";
        return RedirectToAction("Index");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectListing(int id, string? reason)
    {
        var listing = await _context.PetListings
            .Include(l => l.Rehomer)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (listing == null) return NotFound();

        listing.Status          = "Rejected";
        listing.RejectionReason = reason;

        _context.Notifications.Add(new Notification
        {
            UserId    = listing.RehomerId,
            Type      = "listing_rejected",
            Title     = $"Your listing for {listing.Name} needs changes",
            Body      = reason ?? "Your listing was not approved. Please review and resubmit.",
            LinkUrl   = "/Rehome/List",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        TempData["AdminMsg"] = $"{listing.Name}'s listing rejected.";
        return RedirectToAction("Index");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUserRole(string userId, string role)
    {
        if (userId == _userManager.GetUserId(User))
        {
            TempData["AdminMsg"] = "You can't change your own role.";
            return RedirectToAction("Index");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, role);

        user.UserType = role;
        await _userManager.UpdateAsync(user);

        TempData["AdminMsg"] = $"{user.DisplayName}'s role changed to {role}.";
        return RedirectToAction("Index");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUserActive(string userId)
    {
        if (userId == _userManager.GetUserId(User))
        {
            TempData["AdminMsg"] = "You can't deactivate your own account.";
            return RedirectToAction("Index");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var isCurrentlyLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow;
        if (isCurrentlyLocked)
        {
            user.LockoutEnd = null;
            TempData["AdminMsg"] = $"{user.DisplayName}'s account was reactivated.";
        }
        else
        {
            user.LockoutEnabled = true;
            user.LockoutEnd     = DateTimeOffset.MaxValue;
            TempData["AdminMsg"] = $"{user.DisplayName}'s account was deactivated.";
        }
        await _userManager.UpdateAsync(user);
        return RedirectToAction("Index");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateApplicationStatus(int id, string status)
    {
        var app = await _context.AdoptionApplications
            .Include(a => a.Adopter)
            .Include(a => a.PetListing)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app == null) return NotFound();

        var (success, error) = await _appService.ApplyStatusChangeAsync(app, status);
        if (!success)
            return Json(new { success = false, message = error });

        return Json(new { success = true });
    }
}
