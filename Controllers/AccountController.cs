using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PawdoptApp.Models;
using System.Security.Claims;

namespace PawdoptApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser>  _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser>  userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager  = userManager;
        _signInManager = signInManager;
    }

    // ── GET /Account/Login ────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // ── POST /Account/Login ───────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("AdopterProfile", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    // ── GET /Account/Register ─────────────────────────────────────────────
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View();
    }

    // ── POST /Account/Register ────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName       = model.Email,
            Email          = model.Email,
            DisplayName    = model.DisplayName,
            UserType       = model.UserType,
            City           = model.City,
            Province       = "QC",
            EmailConfirmed = true,
            CreatedAt      = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var role = model.UserType == "Rehomer" ? "Rehomer" : "Adopter";
            await _userManager.AddToRoleAsync(user, role);
            await _userManager.AddClaimsAsync(user, new[]
            {
                new Claim("DisplayName", user.DisplayName),
                new Claim("UserType",    user.UserType)
            });

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("AdopterProfile", "Home");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    // ── POST /Account/Logout ──────────────────────────────────────────────
    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
