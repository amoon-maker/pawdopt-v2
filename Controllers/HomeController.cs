using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PawdoptApp.Models;

namespace PawdoptApp.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Adopt()
    {
        return View();
    }

    public IActionResult PetDetail(int id = 1)
    {
        ViewData["PetId"] = id;
        return View();
    }

    [Authorize]
    public async Task<IActionResult> AdopterProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserDisplayName"] = user?.DisplayName ?? User.Identity?.Name ?? "User";
        ViewData["UserEmail"]       = user?.Email ?? "";
        ViewData["UserType"]        = user?.UserType ?? "Adopter";
        ViewData["UserCity"]        = user?.City ?? "";
        ViewData["UserProvince"]    = user?.Province ?? "QC";
        ViewData["MemberSince"]     = user?.CreatedAt.ToString("MMMM yyyy") ?? "2026";
        ViewData["UserInitial"]     = (user?.DisplayName ?? "U").Substring(0, 1).ToUpper();
        return View();
    }

    [Authorize]
    public IActionResult AdoptionWizard(int petId = 1)
    {
        ViewData["PetId"] = petId;
        return View();
    }

    public IActionResult CareGuide()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
