using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Data;
using PawdoptApp.Models;
using System.Security.Claims;

namespace PawdoptApp.ViewComponents;

public class NotificationBellViewComponent : ViewComponent
{
    private readonly ApplicationDbContext        _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public NotificationBellViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context     = context;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return View(new NotificationBellModel());

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(8)
            .ToListAsync();

        var model = new NotificationBellModel
        {
            UnreadCount   = notifications.Count(n => !n.IsRead),
            Notifications = notifications
        };
        return View(model);
    }
}

public class NotificationBellModel
{
    public int                  UnreadCount   { get; set; }
    public List<Notification>   Notifications { get; set; } = new();
}
