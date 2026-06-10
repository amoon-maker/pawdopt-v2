using Microsoft.AspNetCore.Identity;

namespace PawdoptApp.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string UserType { get; set; } = "Adopter"; // Adopter | Rehomer | Admin
    public string? City { get; set; }
    public string Province { get; set; } = "QC";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
