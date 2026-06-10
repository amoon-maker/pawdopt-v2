using System.ComponentModel.DataAnnotations.Schema;

namespace PawdoptApp.Models;

public class PetListing
{
    public int Id { get; set; }
    public string RehomerId { get; set; } = string.Empty;
    public ApplicationUser Rehomer { get; set; } = null!;

    // Step 1 — Basic info
    public string Name       { get; set; } = string.Empty;
    public string Species    { get; set; } = "dog";   // dog | cat | small
    public string Breed      { get; set; } = string.Empty;
    public string Age        { get; set; } = string.Empty;   // "2 years" | "6 months"
    public string Gender     { get; set; } = "male";
    public string Size       { get; set; } = "medium";
    public string? Color     { get; set; }
    public decimal? Weight   { get; set; }
    public string Microchipped { get; set; } = "unknown"; // yes | no | unknown
    public string? Reason    { get; set; }

    // Step 3 — Character & health
    public string? TraitsJson      { get; set; }  // JSON array of trait keys
    public string? CharacterNotes  { get; set; }
    public string Vaccinated       { get; set; } = "unknown"; // yes | partial | no | unknown
    public string Sterilized       { get; set; } = "unknown"; // yes | no | unknown
    public string ActivityLevel    { get; set; } = "moderate"; // low | moderate | high
    public string TrainingLevel    { get; set; } = "basic";    // none | basic | well
    public string IdealHome        { get; set; } = "any";      // apartment | house | any
    public string? HealthNotes     { get; set; }

    // Step 5 — Location + story
    public string? City        { get; set; }
    public string Province     { get; set; } = "QC";
    public string? PostalCode  { get; set; }
    public string? PickupType  { get; set; }  // home | neutral | adopter
    public string? PickupNotes { get; set; }
    public string? Story       { get; set; }

    // Step 2 — Photos
    public bool HasPhotos { get; set; }
    [NotMapped]
    public string? PhotoUrlsJson { get; set; }

    // Step 7 — Documents
    public string? DocumentsJson { get; set; }  // JSON array of doc type keys

    // Lifecycle
    public string    Status          { get; set; } = "Draft";  // Draft | Pending | Approved | Rejected | Adopted
    public DateTime  CreatedAt       { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt     { get; set; }
    public DateTime? ApprovedAt      { get; set; }
    public string?   RejectionReason { get; set; }
    public string?   AdminNote       { get; set; }

    public ICollection<AdoptionApplication> Applications { get; set; } = new List<AdoptionApplication>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
