using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Data;
using PawdoptApp.Models;
using System.Text.Json;

namespace PawdoptApp.Controllers;

public class RehomeController : Controller
{
    private readonly ApplicationDbContext         _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment          _env;

    public RehomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
    {
        _context     = context;
        _userManager = userManager;
        _env         = env;
    }

    // ── Public landing page ───────────────────────────────────────────────
    public IActionResult List() => View();

    public IActionResult Advisor(string id)
    {
        ViewData["AdvisorId"] = id ?? "sophie-martin";
        return View();
    }

    // ── Helper: fetch user's own listing if it's still editable ───────────
    // Draft = never published. Rejected = sent back by an admin and can be fixed & resubmitted.
    private async Task<PetListing?> GetDraft(int id)
    {
        var userId = _userManager.GetUserId(User);
        return await _context.PetListings
            .FirstOrDefaultAsync(l => l.Id == id && l.RehomerId == userId && (l.Status == "Draft" || l.Status == "Rejected"));
    }

    private async Task NotifyAdmins(string type, string title, string body, string link)
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        foreach (var admin in admins)
        {
            _context.Notifications.Add(new Notification
            {
                UserId    = admin.Id,
                Type      = type,
                Title     = title,
                Body      = body,
                LinkUrl   = link,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    // ════════════════════════════════════════════════════════════════════
    // STEP 1 — Basic info
    // ════════════════════════════════════════════════════════════════════

    [Authorize]
    public async Task<IActionResult> NewListing(int? id)
    {
        var listing = id.HasValue ? await GetDraft(id.Value) : null;
        return View(listing);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveStep1(
        int? id, string petName, string species, string breed, string age,
        string gender, string size, string? color, decimal? weight,
        string? microchipped, string? reason)
    {
        PetListing listing;
        if (id.HasValue)
        {
            var existing = await GetDraft(id.Value);
            if (existing == null) return RedirectToAction("NewListing");
            listing = existing;
        }
        else
        {
            listing = new PetListing
            {
                RehomerId = _userManager.GetUserId(User)!,
                Status    = "Draft",
                CreatedAt = DateTime.UtcNow
            };
            _context.PetListings.Add(listing);
        }

        listing.Name         = petName ?? string.Empty;
        listing.Species      = species  ?? "dog";
        listing.Breed        = breed    ?? string.Empty;
        listing.Age          = age      ?? string.Empty;
        listing.Gender       = gender   ?? "male";
        listing.Size         = size     ?? "medium";
        listing.Color        = color;
        listing.Weight       = weight;
        listing.Microchipped = microchipped ?? "unknown";
        listing.Reason       = reason;

        await _context.SaveChangesAsync();
        return RedirectToAction("Images", new { id = listing.Id });
    }

    // ════════════════════════════════════════════════════════════════════
    // STEP 2 — Photos
    // ════════════════════════════════════════════════════════════════════

    [Authorize]
    public async Task<IActionResult> Images(int id)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");
        ViewData["ListingId"] = id;
        return View();
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveStep2(int id, List<IFormFile>? photos)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");

        if (photos != null && photos.Count > 0)
        {
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "listings", id.ToString());
            Directory.CreateDirectory(uploadDir);

            foreach (var file in photos.Take(3))
            {
                if (file.Length == 0) continue;
                var ext      = Path.GetExtension(file.FileName).ToLowerInvariant();
                var filename = Guid.NewGuid().ToString("N") + ext;
                var fullPath = Path.Combine(uploadDir, filename);
                using var stream = System.IO.File.Create(fullPath);
                await file.CopyToAsync(stream);
            }

            listing.HasPhotos = true;
        }
        else
        {
            listing.HasPhotos = true;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Character", new { id });
    }

    // ════════════════════════════════════════════════════════════════════
    // STEP 3 — Character & health
    // ════════════════════════════════════════════════════════════════════

    [Authorize]
    public async Task<IActionResult> Character(int id)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");
        ViewData["ListingId"] = id;
        return View(listing);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveStep3(
        int id, string[]? traits, string? characterNotes, string? vaccinated,
        string? sterilized, string? activityLevel, string? trainingLevel,
        string? idealHome, string? healthNotes)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");

        listing.TraitsJson     = traits != null && traits.Length > 0
            ? System.Text.Json.JsonSerializer.Serialize(traits)
            : null;
        listing.CharacterNotes = characterNotes;
        listing.Vaccinated     = vaccinated     ?? listing.Vaccinated;
        listing.Sterilized     = sterilized     ?? listing.Sterilized;
        listing.ActivityLevel  = activityLevel  ?? listing.ActivityLevel;
        listing.TrainingLevel  = trainingLevel  ?? listing.TrainingLevel;
        listing.IdealHome      = idealHome      ?? listing.IdealHome;
        listing.HealthNotes    = healthNotes;
        await _context.SaveChangesAsync();
        return RedirectToAction("Location", new { id });
    }

    // KeyFacts is part of the extended flow — just passes through to Location
    [Authorize]
    public async Task<IActionResult> KeyFacts(int id)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");
        ViewData["ListingId"] = id;
        return View(listing);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveStep4(
        int id, string? vaccinated, string? sterilized, string? activityLevel,
        string? trainingLevel, string? idealHome, string? healthNotes)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");
        if (!string.IsNullOrEmpty(vaccinated))    listing.Vaccinated    = vaccinated;
        if (!string.IsNullOrEmpty(sterilized))    listing.Sterilized    = sterilized;
        if (!string.IsNullOrEmpty(activityLevel)) listing.ActivityLevel = activityLevel;
        if (!string.IsNullOrEmpty(trainingLevel)) listing.TrainingLevel = trainingLevel;
        if (!string.IsNullOrEmpty(idealHome))     listing.IdealHome     = idealHome;
        if (!string.IsNullOrEmpty(healthNotes))   listing.HealthNotes   = healthNotes;
        await _context.SaveChangesAsync();
        return RedirectToAction("Location", new { id });
    }

    // ════════════════════════════════════════════════════════════════════
    // STEP 5 — Location + story
    // ════════════════════════════════════════════════════════════════════

    [Authorize]
    public async Task<IActionResult> Location(int id)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");
        ViewData["ListingId"] = id;
        return View(listing);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveStep5(
        int id, string? city, string? province, string? postalCode,
        string? pickupType, string? pickupNotes)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");
        listing.City        = city;
        listing.Province    = province    ?? "QC";
        listing.PostalCode  = postalCode;
        listing.PickupType  = pickupType;
        listing.PickupNotes = pickupNotes;
        await _context.SaveChangesAsync();
        return RedirectToAction("Story", new { id });
    }

    // ════════════════════════════════════════════════════════════════════
    // STEP 6 — Story (standalone, can also update story field)
    // ════════════════════════════════════════════════════════════════════

    [Authorize]
    public async Task<IActionResult> Story(int id)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");
        ViewData["ListingId"] = id;
        return View(listing);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveStep6(int id, string? story)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");
        if (!string.IsNullOrEmpty(story)) listing.Story = story;
        await _context.SaveChangesAsync();
        return RedirectToAction("Documents", new { id });
    }

    // ════════════════════════════════════════════════════════════════════
    // STEP 7 — Documents
    // ════════════════════════════════════════════════════════════════════

    [Authorize]
    public async Task<IActionResult> Documents(int id)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");
        ViewData["ListingId"] = id;
        return View(listing);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveStep7(int id, string? documentsJson)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");
        listing.DocumentsJson = documentsJson;
        await _context.SaveChangesAsync();
        return RedirectToAction("Confirm", new { id });
    }

    // ════════════════════════════════════════════════════════════════════
    // STEP 8 — Confirm & Publish
    // ════════════════════════════════════════════════════════════════════

    [Authorize]
    public async Task<IActionResult> Confirm(int id)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");

        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "listings", id.ToString());
        if (Directory.Exists(uploadDir))
        {
            var files = Directory.GetFiles(uploadDir)
                .Select(f => $"/uploads/listings/{id}/{Path.GetFileName(f)}")
                .ToList();
            listing.PhotoUrlsJson = files.Count > 0 ? JsonSerializer.Serialize(files) : null;
        }

        return View(listing);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        var listing = await GetDraft(id);
        if (listing == null) return RedirectToAction("NewListing");

        listing.Status          = "Pending";
        listing.PublishedAt      = DateTime.UtcNow;
        listing.RejectionReason  = null;
        listing.AdminNote        = null;

        await NotifyAdmins(
            type:  "new_listing",
            title: $"New listing awaiting approval: {listing.Name}",
            body:  $"A rehomer has submitted a new listing for {listing.Name} ({listing.Breed}). Please review.",
            link:  "/Admin"
        );

        await _context.SaveChangesAsync();
        return RedirectToAction("Success");
    }

    [Authorize]
    public IActionResult Success() => View();

    // ════════════════════════════════════════════════════════════════════
    // DELETE — owner only, blocked while a real application is in progress
    // ════════════════════════════════════════════════════════════════════

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteListing(int id)
    {
        var userId = _userManager.GetUserId(User);
        var listing = await _context.PetListings
            .FirstOrDefaultAsync(l => l.Id == id && l.RehomerId == userId);
        if (listing == null) return NotFound();

        var hasActiveApplications = await _context.AdoptionApplications
            .AnyAsync(a => a.PetListingId == id && a.Status != "Withdrawn" && a.Status != "Rejected");
        if (hasActiveApplications)
        {
            TempData["ListingError"] = $"Can't delete \"{listing.Name}\" — it has active adoption applications.";
            return RedirectToAction("AdopterProfile", "Home", new { tab = "listings" });
        }

        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "listings", id.ToString());
        if (Directory.Exists(uploadDir)) Directory.Delete(uploadDir, recursive: true);

        _context.PetListings.Remove(listing);
        await _context.SaveChangesAsync();
        TempData["ListingMsg"] = $"\"{listing.Name}\" was deleted.";
        return RedirectToAction("AdopterProfile", "Home", new { tab = "listings" });
    }
}
