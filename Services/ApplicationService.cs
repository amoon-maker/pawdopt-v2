using Microsoft.EntityFrameworkCore;
using PawdoptApp.Data;
using PawdoptApp.Models;

namespace PawdoptApp.Services;

public class ApplicationService
{
    private readonly ApplicationDbContext _context;

    public ApplicationService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Validates the transition, applies the new status, cancels competitors on approval,
    // and writes all notifications — all inside a DB transaction when approving.
    // Caller must load `app` with .Include(a => a.Adopter).Include(a => a.PetListing).
    // Returns (true, null) on success; (false, errorMessage) on invalid transition or DB error.
    public async Task<(bool Success, string? Error)> ApplyStatusChangeAsync(
        AdoptionApplication app, string newStatus, string? reviewNotes = null)
    {
        if (!app.Status.CanTransitionTo(newStatus))
            return (false, $"Cannot move an application from \"{app.Status.Label()}\" to \"{newStatus.Label()}\".");

        var petName = app.PetListing.Name;

        if (newStatus == nameof(ApplicationStatus.Approved))
        {
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                app.Status      = nameof(ApplicationStatus.Approved);
                app.ReviewedAt  = DateTime.UtcNow;
                app.ReviewNotes = reviewNotes;

                var competing = await _context.AdoptionApplications
                    .Include(a => a.PetListing)
                    .Where(a => a.PetListingId == app.PetListingId &&
                                a.Id != app.Id &&
                                (a.Status == "Pending" || a.Status == "UnderReview"))
                    .ToListAsync();

                foreach (var other in competing)
                {
                    other.Status = nameof(ApplicationStatus.Cancelled);
                    _context.Notifications.Add(new Notification
                    {
                        UserId    = other.AdopterId,
                        Type      = "app_cancelled",
                        Title     = $"Update on your application for {other.PetListing.Name}",
                        Body      = "Another adopter was selected for this pet. Thank you for your interest!",
                        LinkUrl   = "/Applications",
                        CreatedAt = DateTime.UtcNow
                    });
                }

                _context.Notifications.Add(new Notification
                {
                    UserId    = app.AdopterId,
                    Type      = "app_approved",
                    Title     = $"Your application for {petName} was approved!",
                    Body      = string.IsNullOrWhiteSpace(reviewNotes)
                                    ? "Congratulations! The rehomer has selected you. They will be in touch soon."
                                    : reviewNotes,
                    LinkUrl   = "/Applications",
                    CreatedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                return (false, "Something went wrong. Please try again.");
            }
        }
        else
        {
            app.Status      = newStatus;
            app.ReviewedAt  = DateTime.UtcNow;
            app.ReviewNotes = reviewNotes;

            var notifBody = newStatus == nameof(ApplicationStatus.Rejected)
                ? (string.IsNullOrWhiteSpace(reviewNotes)
                       ? "Unfortunately your application was not selected."
                       : reviewNotes)
                : $"Your application status changed to: {newStatus.Label()}.";

            _context.Notifications.Add(new Notification
            {
                UserId    = app.AdopterId,
                Type      = "app_status_change",
                Title     = $"Update on your application for {petName}",
                Body      = notifBody,
                LinkUrl   = "/Applications",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        return (true, null);
    }
}
