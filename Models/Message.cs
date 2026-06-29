namespace PawdoptApp.Models;

public class Message
{
    public int Id { get; set; }
    public string SenderId   { get; set; } = string.Empty;
    public ApplicationUser Sender   { get; set; } = null!;
    public string ReceiverId { get; set; } = string.Empty;
    public ApplicationUser Receiver { get; set; } = null!;

    public int? RelatedListingId      { get; set; }
    public PetListing? RelatedListing { get; set; }

    public string Body   { get; set; } = string.Empty;
    public bool   IsRead { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    // "Delete conversation" only hides messages for the side that deleted them —
    // the other participant's inbox is unaffected.
    public bool DeletedForSender   { get; set; }
    public bool DeletedForReceiver { get; set; }
}
