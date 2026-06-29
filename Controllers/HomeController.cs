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
    private readonly IWebHostEnvironment          _env;

    // Real PetListing ids are offset in the client-side pet grid so they never
    // collide with the hardcoded demo pet ids (1-12) from pawdopt-data.js.
    private const int RealPetIdOffset = 10000;

    public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _context     = context;
        _env         = env;
    }

    public IActionResult Index()   => View();

    public async Task<IActionResult> Adopt()
    {
        ViewData["RealPets"] = await GetApprovedPetDtosAsync();
        return View();
    }

    public IActionResult Privacy() => View();
    public IActionResult About() => View();
    public IActionResult CareGuide() => View();

    public async Task<IActionResult> PetDetail(int id = 1)
    {
        ViewData["PetId"]    = id;
        ViewData["RealPets"] = await GetApprovedPetDtosAsync();

        if (User.Identity?.IsAuthenticated == true)
        {
            var realId = id - RealPetIdOffset;
            if (realId > 0)
            {
                var userId = _userManager.GetUserId(User);
                var existing = await _context.AdoptionApplications
                    .Where(a => a.AdopterId == userId &&
                                a.PetListingId == realId &&
                                a.Status != "Withdrawn")
                    .OrderByDescending(a => a.SubmittedAt)
                    .Select(a => new { a.Id, a.Status })
                    .FirstOrDefaultAsync();

                if (existing != null)
                {
                    ViewData["ExistingAppId"]     = existing.Id;
                    ViewData["ExistingAppStatus"] = existing.Status;
                }
            }
        }

        return View();
    }

    // ── Real, admin-approved listings shaped to match window.PAWDOPT_PETS ──
    private async Task<List<object>> GetApprovedPetDtosAsync()
    {
        var approved = await _context.PetListings
            .Include(l => l.Rehomer)
            .Where(l => l.Status == "Approved")
            .OrderByDescending(l => l.ApprovedAt)
            .ToListAsync();

        return approved.Select(BuildPetDto).ToList();
    }

    private object BuildPetDto(PetListing l)
    {
        var (ageNum, ageUnit) = ParseAge(l.Age);

        var photos = new List<string>();
        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "listings", l.Id.ToString());
        if (Directory.Exists(uploadDir))
        {
            photos = Directory.GetFiles(uploadDir)
                .Select(f => $"/uploads/listings/{l.Id}/{Path.GetFileName(f)}")
                .ToList();
        }

        var traits = new List<string>();
        if (!string.IsNullOrEmpty(l.TraitsJson))
        {
            try { traits = JsonSerializer.Deserialize<List<string>>(l.TraitsJson) ?? new(); }
            catch (JsonException) { }
        }

        return new
        {
            id           = l.Id + RealPetIdOffset,
            realId       = l.Id,
            name         = l.Name,
            species      = l.Species,
            breed        = string.IsNullOrEmpty(l.Breed) ? "Mixed" : l.Breed,
            size         = l.Size,
            ageNum,
            ageUnit,
            gender       = l.Gender,
            province     = l.Province,
            city         = l.City ?? "",
            image        = photos.FirstOrDefault(),
            emoji        = l.Species == "cat" ? "🐱" : "🐶",
            badge        = l.ApprovedAt.HasValue && l.ApprovedAt > DateTime.UtcNow.AddDays(-14) ? "new" : "",
            vaccinated   = l.Vaccinated == "yes",
            neutered     = l.Sterilized == "yes",
            microchipped = l.Microchipped == "yes",
            colors       = "linear-gradient(135deg,#e8f0e8 0%,#d8eef5 100%)",
            desc         = !string.IsNullOrEmpty(l.CharacterNotes) ? l.CharacterNotes : (l.Story ?? ""),
            story        = l.Story ?? "",
            traits,
            rehomer = new
            {
                id     = l.RehomerId,
                name   = l.Rehomer.DisplayName,
                city   = string.IsNullOrEmpty(l.City) ? l.Province : $"{l.City}, {l.Province}",
                since  = l.Rehomer.CreatedAt.ToString("MMMM yyyy"),
                avatar = string.IsNullOrEmpty(l.Rehomer.DisplayName) ? "?" : l.Rehomer.DisplayName.Substring(0, 1).ToUpper()
            }
        };
    }

    private static (int num, string unit) ParseAge(string? age)
    {
        var unit = !string.IsNullOrEmpty(age) && age.Contains("mo", StringComparison.OrdinalIgnoreCase) ? "mo" : "yr";
        if (string.IsNullOrWhiteSpace(age)) return (0, unit);

        var digits = new string(age.TakeWhile(char.IsDigit).ToArray());
        int.TryParse(digits, out var num);
        return (num, unit);
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
        ViewData["RealPets"]        = await GetApprovedPetDtosAsync();

        // Real applications for this user
        var userId = _userManager.GetUserId(User);
        var apps = await _context.AdoptionApplications
            .Include(a => a.PetListing)
            .Where(a => a.AdopterId == userId)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();
        ViewData["Applications"] = apps;

        // Real listings + incoming applications if rehomer
        var hasListings = false;
        if (user?.UserType == "Rehomer" || user?.UserType == "Admin")
        {
            var listings = await _context.PetListings
                .Where(l => l.RehomerId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
            ViewData["Listings"] = listings;
            hasListings = listings.Count > 0;

            var listingIds = listings.Select(l => l.Id).ToList();
            var incomingApps = await _context.AdoptionApplications
                .Include(a => a.PetListing)
                .Include(a => a.Adopter)
                .Where(a => listingIds.Contains(a.PetListingId))
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();
            ViewData["IncomingApplications"] = incomingApps;
        }

        // Profile completion: name + city + phone are optional fields the user can fill in,
        // plus credit for actually using the platform (an application or a listing).
        var completionChecks = new[]
        {
            !string.IsNullOrWhiteSpace(user?.DisplayName),
            !string.IsNullOrWhiteSpace(user?.City),
            !string.IsNullOrWhiteSpace(user?.PhoneNumber),
            apps.Count > 0 || hasListings
        };
        ViewData["ProfileCompletion"] = (int)Math.Round(100.0 * completionChecks.Count(c => c) / completionChecks.Length);

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

        // Notify the rehomer — someone wants to adopt their pet
        var adopter = await _userManager.GetUserAsync(User);
        _context.Notifications.Add(new Notification
        {
            UserId    = listing.RehomerId,
            Type      = "new_application",
            Title     = $"New application for {listing.Name}",
            Body      = $"{adopter?.DisplayName ?? "An adopter"} applied to adopt {listing.Name}. Review their application now.",
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
