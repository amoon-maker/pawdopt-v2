using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Data;
using PawdoptApp.Models;

namespace PawdoptApp.Controllers;

[Authorize]
public class MessagesController : Controller
{
    private readonly ApplicationDbContext        _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context     = context;
        _userManager = userManager;
    }

    // ── GET /Messages — inbox ─────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        // Group messages by conversation partner, latest message per thread
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.RelatedListing)
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .OrderByDescending(m => m.SentAt)
            .ToListAsync();

        // Build conversation list (one entry per partner)
        var seen      = new HashSet<string>();
        var threads   = new List<Message>();
        foreach (var msg in messages)
        {
            var partnerId = msg.SenderId == userId ? msg.ReceiverId : msg.SenderId;
            if (seen.Add(partnerId)) threads.Add(msg);
        }

        ViewData["UserId"] = userId;
        return View(threads);
    }

    // ── GET /Messages/Conversation?with=userId ────────────────────────────
    public async Task<IActionResult> Conversation(string with, int? listingId = null)
    {
        var userId  = _userManager.GetUserId(User)!;
        var partner = await _userManager.FindByIdAsync(with);
        if (partner == null) return NotFound();

        var msgs = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.RelatedListing)
            .Where(m => (m.SenderId == userId && m.ReceiverId == with)
                     || (m.SenderId == with   && m.ReceiverId == userId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        // Mark unread messages from partner as read
        var unread = msgs.Where(m => m.ReceiverId == userId && !m.IsRead).ToList();
        unread.ForEach(m => m.IsRead = true);
        if (unread.Count > 0) await _context.SaveChangesAsync();

        ViewData["PartnerId"]   = with;
        ViewData["PartnerName"] = partner.DisplayName;
        ViewData["UserId"]      = userId;
        ViewData["ListingId"]   = listingId;
        return View(msgs);
    }

    // ── POST /Messages/Send ───────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(string receiverId, string body, int? listingId)
    {
        var userId = _userManager.GetUserId(User)!;
        if (string.IsNullOrWhiteSpace(body)) return BadRequest();

        var sender = await _userManager.GetUserAsync(User);

        var msg = new Message
        {
            SenderId         = userId,
            ReceiverId       = receiverId,
            Body             = body.Trim(),
            RelatedListingId = listingId,
            IsRead           = false,
            SentAt           = DateTime.UtcNow
        };
        _context.Messages.Add(msg);

        // Notify receiver
        _context.Notifications.Add(new Notification
        {
            UserId    = receiverId,
            Type      = "new_message",
            Title     = $"New message from {sender?.DisplayName ?? "someone"}",
            Body      = body.Length > 80 ? body[..77] + "…" : body,
            LinkUrl   = $"/Messages/Conversation?with={userId}",
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return RedirectToAction("Conversation", new { with = receiverId, listingId });
    }
}
