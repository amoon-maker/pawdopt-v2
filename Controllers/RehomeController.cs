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
        return RedirectToAction("Images");
    }
}
