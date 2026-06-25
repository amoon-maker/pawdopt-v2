namespace PawdoptApp.Models;

public class AdoptionApplication
{
    public int Id { get; set; }

    // ── Who is applying ──────────────────────────────────────────────────────
    public string AdopterId { get; set; } = string.Empty;
    public ApplicationUser Adopter { get; set; } = null!;

    // ── Which pet (required real listing — no hardcoded IDs) ─────────────────
    public int PetListingId { get; set; }
    public PetListing PetListing { get; set; } = null!;

    // ── Contact info collected in wizard step 1 ───────────────────────────────
    // LastName, Email, Phone, City are kept because they may differ from
    // the adopter's account values and are meaningful for this application.
    public string LastName { get; set; } = string.Empty;
    public string Email    { get; set; } = string.Empty;
    public string? Phone   { get; set; }
    public string? City    { get; set; }

    // ── Experience ────────────────────────────────────────────────────────────
    public string? PrevPets     { get; set; }   // yes | no
    public string? PrevPetTypes { get; set; }
    public string? ExpLevel     { get; set; }   // first | some | experienced
    public string? WhyAdopt     { get; set; }

    // ── Home environment ──────────────────────────────────────────────────────
    public string? HomeType           { get; set; }   // house | apartment | condo | other
    public string? Outdoor            { get; set; }   // fenced | patio | none
    public string? Ownership          { get; set; }   // own | rent
    public string? LandlordPermission { get; set; }   // yes | no | na
    public string? Household          { get; set; }   // 1 | 2 | 3+
    public string? Children           { get; set; }   // yes | no
    public string? ChildAgesJson      { get; set; }   // JSON array

    // ── Lifestyle ─────────────────────────────────────────────────────────────
    public string? OtherPets       { get; set; }   // yes | no
    public string? OtherPetDetails { get; set; }
    public string? HoursAlone      { get; set; }   // lt2 | 2to4 | 4to6 | 6to8 | gt8
    public string? Activity        { get; set; }   // sedentary | light | moderate | active
    public string? BreedExp        { get; set; }   // yes | no | research
    public string? AllOnBoard      { get; set; }   // yes | mostly | not-yet
    public string? Notes           { get; set; }

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    public string    Status      { get; set; } = "Pending";
    public DateTime  SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt  { get; set; }
    public string?   ReviewNotes { get; set; }
}
