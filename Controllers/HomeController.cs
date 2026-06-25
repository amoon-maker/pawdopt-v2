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
        ViewData["UserPhone"]       = user?.PhoneNumber ?? "";
        ViewData["MemberSince"]     = user?.CreatedAt.ToString("MMMM yyyy") ?? "2026";
        ViewData["UserInitial"]     = (user?.DisplayName ?? "U").Substring(0, 1).ToUpper();
        ViewData["ActiveTab"]       = Request.Query["tab"].ToString();

        // Real applications for this user
        var userId = _userManager.GetUserId(User);
        var apps = await _context.AdoptionApplications
            .Include(a => a.PetListing)
            .Where(a => a.AdopterId == userId)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();
        ViewData["Applications"] = apps;

        // Real listings + incoming applications if rehomer
        if (user?.UserType == "Rehomer" || user?.UserType == "Admin")
        {
            var listings = await _context.PetListings
                .Where(l => l.RehomerId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
            ViewData["Listings"] = listings;

            var listingIds = listings.Select(l => l.Id).ToList();
            var incomingApps = await _context.AdoptionApplications
                .Include(a => a.PetListing)
                .Include(a => a.Adopter)
                .Where(a => listingIds.Contains(a.PetListingId))
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();
            ViewData["IncomingApplications"] = incomingApps;
        }

        return View();
    }

    [Authorize]
    public async Task<IActionResult> AdoptionWizard(int petListingId)
    {
        var listing = await _context.PetListings
            .Include(l => l.Rehomer)
            .FirstOrDefaultAsync(l => l.Id == petListingId && l.Status == "Approved");
        if (listing == null) return NotFound();
        ViewData["Listing"] = listing;
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

        var petListingId = GetInt("petListingId");
        var listing = await _context.PetListings.FindAsync(petListingId);
        if (listing == null)
            return Json(new { success = false, message = "Pet listing not found." });

        // Prevent duplicate submissions for the same listing
        var duplicate = await _context.AdoptionApplications
            .AnyAsync(a => a.AdopterId == userId && a.PetListingId == petListingId && a.Status != "Withdrawn");
        if (duplicate)
            return Json(new { success = false, message = "You have already applied for this pet." });

        // Serialize child ages array
        string? childAgesJson = null;
        if (body.TryGetProperty("childAges", out var ca) && ca.ValueKind == JsonValueKind.Array)
            childAgesJson = ca.GetRawText();

        var app = new AdoptionApplication
        {
            AdopterId          = userId,
            PetListingId       = petListingId,
            LastName           = Get("lastName"),
            Email              = Get("email"),
            Phone              = Get("phone"),
            City               = Get("city"),
            PrevPets           = Get("prevPets"),
            PrevPetTypes       = Get("prevPetTypes"),
            ExpLevel           = Get("expLevel"),
            WhyAdopt           = Get("whyAdopt"),
            HomeType           = Get("homeType"),
            Outdoor            = Get("outdoor"),
            Ownership          = Get("ownership"),
            LandlordPermission = Get("landlord"),
            Household          = Get("household"),
            Children           = Get("children"),
            ChildAgesJson      = childAgesJson,
            OtherPets          = Get("otherPets"),
            OtherPetDetails    = Get("otherPetDetails"),
            HoursAlone         = Get("hoursAlone"),
            Activity           = Get("activity"),
            BreedExp           = Get("breedExp"),
            AllOnBoard         = Get("allOnBoard"),
            Notes              = Get("notes"),
            Status             = "Pending",
            SubmittedAt        = DateTime.UtcNow
        };
        _context.AdoptionApplications.Add(app);

        // Notify adopter — pet name comes from the real listing
        _context.Notifications.Add(new Notification
        {
            UserId    = userId,
            Type      = "app_submitted",
            Title     = $"Application submitted for {listing.Name}",
            Body      = "Your application is under review. We'll notify you when there's an update.",
            LinkUrl   = "/Applications",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return Json(new { success = true, applicationId = app.Id });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> UpdateProfile(string displayName, string city, string province, string phone)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.DisplayName = displayName?.Trim() ?? user.DisplayName;
        user.City        = city?.Trim();
        user.Province    = province?.Trim() ?? user.Province;
        user.PhoneNumber = phone?.Trim();

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
            TempData["ProfileSuccess"] = "Profile updated successfully.";
        else
            TempData["ProfileError"] = string.Join(" ", result.Errors.Select(e => e.Description));

        return RedirectToAction("AdopterProfile", new { tab = "settings" });
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            TempData["PasswordError"] = "New passwords do not match.";
            return RedirectToAction("AdopterProfile", new { tab = "settings" });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
            TempData["PasswordSuccess"] = "Password changed successfully.";
        else
            TempData["PasswordError"] = string.Join(" ", result.Errors.Select(e => e.Description));

        return RedirectToAction("AdopterProfile", new { tab = "settings" });
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
