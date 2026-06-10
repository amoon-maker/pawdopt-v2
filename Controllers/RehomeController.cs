using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PawdoptApp.Controllers;

public class RehomeController : Controller
{
    // Public info/landing page
    public IActionResult List()
    {
        return View();
    }

    // Community advisor profile — public
    public IActionResult Advisor(string id)
    {
        ViewData["AdvisorId"] = id ?? "sophie-martin";
        return View();
    }

    // All wizard steps require login
    [Authorize]
    public IActionResult NewListing()
    {
        return View();
    }

    [HttpPost, Authorize]
    public IActionResult SaveStep1()
    {
        return RedirectToAction("Images");
    }

    [Authorize]
    public IActionResult Images()
    {
        return View();
    }

    [HttpPost, Authorize]
    public IActionResult SaveStep2()
    {
        return RedirectToAction("Character");
    }

    [Authorize]
    public IActionResult Character()
    {
        return View();
    }

    [HttpPost, Authorize]
    public IActionResult SaveStep3()
    {
        return RedirectToAction("Location");
    }

    [Authorize]
    public IActionResult KeyFacts()
    {
        return View();
    }

    [HttpPost, Authorize]
    public IActionResult SaveStep4()
    {
        return RedirectToAction("Location");
    }

    [Authorize]
    public IActionResult Location()
    {
        return View();
    }

    [HttpPost, Authorize]
    public IActionResult SaveStep5()
    {
        return RedirectToAction("Confirm");
    }

    [Authorize]
    public IActionResult Story()
    {
        return View();
    }

    [HttpPost, Authorize]
    public IActionResult SaveStep6()
    {
        return RedirectToAction("Documents");
    }

    [Authorize]
    public IActionResult Documents()
    {
        return View();
    }

    [HttpPost, Authorize]
    public IActionResult SaveStep7()
    {
        return RedirectToAction("Confirm");
    }

    [Authorize]
    public IActionResult Confirm()
    {
        return View();
    }

    [HttpPost, Authorize]
    public IActionResult Publish()
    {
        return RedirectToAction("Success");
    }

    [Authorize]
    public IActionResult Success()
    {
        return View();
    }
}
