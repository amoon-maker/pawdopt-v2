using System.ComponentModel.DataAnnotations;

namespace PawdoptApp.Models;

public class RegisterViewModel
{
    [Required, Display(Name = "Full Name")]
    [StringLength(100, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "Password must be at least {2} characters.", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string UserType { get; set; } = "Adopter"; // Adopter | Rehomer

    public string? City { get; set; }
}
