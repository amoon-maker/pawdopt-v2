using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Data;
using PawdoptApp.Models;

namespace PawdoptApp.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly ApplicationDbContext        _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context     = context;
        _userManager = userManager;
    }

    // ── GET /Notifications/Count — JSON for the bell badge ───────────────
    [HttpGet]
    public async Task<IActionResult> Count()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Json(new { count = 0 });

        var count = await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);

        return Json(new { count });
    }

    // ── POST /Notifications/MarkRead/5 ────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        var userId = _userManager.GetUserId(User);
        var notif  = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notif != null)
        {
            notif.IsRead = true;
            await _context.SaveChangesAsync();
        }
        return Json(new { success = true });
    }

    // ── POST /Notifications/MarkAllRead ───────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        var userId = _userManager.GetUserId(User);
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

        return Json(new { success = true });
    }
}
