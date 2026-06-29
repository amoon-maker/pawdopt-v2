namespace PawdoptApp.Models;

public class SavedPet
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    // Client-side pet id: demo dataset (1-12) or real PetListing.Id + 10000 offset.
    // Not a foreign key to PetListing since saved pets can be demo-only.
    public int PetId { get; set; }
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}
