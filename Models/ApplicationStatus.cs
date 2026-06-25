namespace PawdoptApp.Models;

/// <summary>
/// Stored as a string in the database so existing rows are unaffected.
/// Convert with app.Status == ApplicationStatus.Approved.ToString()
/// </summary>
public enum ApplicationStatus
{
    Pending,       // just submitted, not yet reviewed
    UnderReview,   // rehomer has opened it
    Approved,      // adoption confirmed
    Rejected,      // declined by rehomer / admin
    Withdrawn,     // cancelled by the adopter themselves
    Cancelled      // automatically cancelled when another app for the same pet was approved
}

public static class ApplicationStatusExtensions
{
    private static readonly Dictionary<string, string[]> _allowed = new()
    {
        [nameof(ApplicationStatus.Pending)]     = [nameof(ApplicationStatus.UnderReview), nameof(ApplicationStatus.Rejected)],
        [nameof(ApplicationStatus.UnderReview)] = [nameof(ApplicationStatus.Approved),    nameof(ApplicationStatus.Rejected)],
        [nameof(ApplicationStatus.Approved)]    = [],
        [nameof(ApplicationStatus.Rejected)]    = [],
        [nameof(ApplicationStatus.Withdrawn)]   = [],
        [nameof(ApplicationStatus.Cancelled)]   = [],
    };

    public static bool CanTransitionTo(this string current, string next)
    {
        return _allowed.TryGetValue(current, out var allowed) && allowed.Contains(next);
    }

    public static string BadgeCss(this string status) => status switch
    {
        nameof(ApplicationStatus.Pending)     => "app-badge app-badge-pending",
        nameof(ApplicationStatus.UnderReview) => "app-badge app-badge-review",
        nameof(ApplicationStatus.Approved)    => "app-badge app-badge-approved",
        nameof(ApplicationStatus.Rejected)    => "app-badge app-badge-rejected",
        nameof(ApplicationStatus.Withdrawn)   => "app-badge app-badge-withdrawn",
        nameof(ApplicationStatus.Cancelled)   => "app-badge app-badge-cancelled",
        _                                     => "app-badge",
    };

    public static string Label(this string status) => status switch
    {
        nameof(ApplicationStatus.Pending)     => "Pending",
        nameof(ApplicationStatus.UnderReview) => "Under Review",
        nameof(ApplicationStatus.Approved)    => "Approved",
        nameof(ApplicationStatus.Rejected)    => "Rejected",
        nameof(ApplicationStatus.Withdrawn)   => "Withdrawn",
        nameof(ApplicationStatus.Cancelled)   => "Cancelled",
        _                                     => status,
    };
}
