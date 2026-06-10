using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawdoptApp.Models;

namespace PawdoptApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
        return View(users);
    }
}
