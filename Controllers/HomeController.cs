using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Data;
using PawdoptApp.Models;

namespace PawdoptApp.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext         _context;

    public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context     = context;
    }

    public IActionResult Index()   => View();
    public IActionResult Adopt()   => View();
    public IActionResult Privacy() => View();
    public IActionResult About()   => View();
    public IActionResult CareGuide() => View();

    public IActionResult PetDetail(int id = 1)
    {
        ViewData["PetId"] = id;
        return View();
    }

    [Authorize]
    public async Task<IActionResult> AdopterProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserDisplayName"] = user?.DisplayName ?? User.Identity?.Name ?? "User";
        ViewData["UserEmail"]       = user?.Email ?? "";
        ViewData["UserType"]        = user?.UserType ?? "Adopter";
        ViewData["UserCity"]        = user?.City ?? "";
        ViewData["UserProvince"]    = user?.Province ?? "QC";
        ViewData["MemberSince"]     = user?.CreatedAt.ToString("MMMM yyyy") ?? "2026";
        ViewData["UserInitial"]     = (user?.DisplayName ?? "U").Substring(0, 1).ToUpper();

        // Real applications for this user
        var userId = _userManager.GetUserId(User);
        var apps = await _context.AdoptionApplications
            .Where(a => a.AdopterId == userId)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();
        ViewData["Applications"] = apps;

        // Real listings if rehomer
        if (user?.UserType == "Rehomer" || user?.UserType == "Admin")
        {
            var listings = await _context.PetListings
                .Where(l => l.RehomerId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
            ViewData["Listings"] = listings;
        }

        return View();
    }

    [Authorize]
    public IActionResult AdoptionWizard(int petId = 1)
    {
        ViewData["PetId"] = petId;
        return View();
    }

    // ── POST /Home/SubmitApplication (JSON from wizard JS) ────────────────
    [HttpPost, Authorize]
    public async Task<IActionResult> SubmitApplication([FromBody] JsonElement body)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        string Get(string key) => body.TryGetProperty(key, out var v) ? v.GetString() ?? "" : "";
        int GetInt(string key) => body.TryGetProperty(key, out var v) && v.TryGetInt32(out var i) ? i : 0;

        var petId   = GetInt("petId");
        var petName = Get("petName");

        // Prevent duplicate submissions
        var duplicate = await _context.AdoptionApplications
            .AnyAsync(a => a.AdopterId == userId && a.HardcodedPetId == petId && a.Status != "Withdrawn");
        if (duplicate)
            return Json(new { success = false, message = "You have already applied for this pet." });

        // Serialize child ages array
        string? childAgesJson = null;
        if (body.TryGetProperty("childAges", out var ca) && ca.ValueKind == JsonValueKind.Array)
            childAgesJson = ca.GetRawText();

        var app = new AdoptionApplication
        {
            AdopterId        = userId,
            HardcodedPetId   = petId > 0 ? petId : null,
            PetName          = petName,
            FirstName        = Get("firstName"),
            LastName         = Get("lastName"),
            Email            = Get("email"),
            Phone            = Get("phone"),
            City             = Get("city"),
            PrevPets         = Get("prevPets"),
            PrevPetTypes     = Get("prevPetTypes"),
            ExpLevel         = Get("expLevel"),
            WhyAdopt         = Get("whyAdopt"),
            HomeType         = Get("homeType"),
            Outdoor          = Get("outdoor"),
            Ownership        = Get("ownership"),
            LandlordPermission = Get("landlord"),
            Household        = Get("household"),
            Children         = Get("children"),
            ChildAgesJson    = childAgesJson,
            OtherPets        = Get("otherPets"),
            OtherPetDetails  = Get("otherPetDetails"),
            HoursAlone       = Get("hoursAlone"),
            Activity         = Get("activity"),
            BreedExp         = Get("breedExp"),
            AllOnBoard       = Get("allOnBoard"),
            Notes            = Get("notes"),
            Status           = "Pending",
            SubmittedAt      = DateTime.UtcNow
        };
        _context.AdoptionApplications.Add(app);

        // Notify adopter
        _context.Notifications.Add(new Notification
        {
            UserId    = userId,
            Type      = "app_submitted",
            Title     = $"Application submitted for {petName}",
            Body      = "Your application is under review. We'll notify you when there's an update.",
            LinkUrl   = "/Home/AdopterProfile",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return Json(new { success = true, applicationId = app.Id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> WithdrawApplication(int id)
    {
        var userId = _userManager.GetUserId(User);
        var app = await _context.AdoptionApplications
            .FirstOrDefaultAsync(a => a.Id == id && a.AdopterId == userId);

        if (app == null) return NotFound();
        app.Status = "Withdrawn";
        await _context.SaveChangesAsync();
        return RedirectToAction("AdopterProfile");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
