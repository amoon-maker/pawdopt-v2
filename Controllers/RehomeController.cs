using Microsoft.AspNetCore.Mvc;

namespace PawdoptApp.Controllers;

public class RehomeController : Controller
{
    public IActionResult List()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SaveStep1()
    {
        return RedirectToAction("Images");
    }

    public IActionResult Images()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SaveStep2()
    {
        return RedirectToAction("Character");
    }

    public IActionResult Character()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SaveStep3()
    {
        return RedirectToAction("KeyFacts");
    }

    public IActionResult KeyFacts()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SaveStep4()
    {
        return RedirectToAction("Location");
    }

    public IActionResult Location()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SaveStep5()
    {
        return RedirectToAction("Story");
    }

    public IActionResult Story()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SaveStep6()
    {
        return RedirectToAction("Documents");
    }

    public IActionResult Documents()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SaveStep7()
    {
        return RedirectToAction("Confirm");
    }

    public IActionResult Confirm()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Publish()
    {
        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
    }
}
