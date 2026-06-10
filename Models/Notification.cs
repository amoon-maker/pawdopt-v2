namespace PawdoptApp.Models;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    // new_listing | listing_approved | listing_rejected
    // new_application | app_status_change | new_message
    public string  Type    { get; set; } = string.Empty;
    public string  Title   { get; set; } = string.Empty;
    public string? Body    { get; set; }
    public string? LinkUrl { get; set; }
    public bool    IsRead  { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
