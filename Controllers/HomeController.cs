using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PawdoptApp.Models;

namespace PawdoptApp.Controllers;

public class HomeController : Controller
{
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

    public IActionResult AdopterProfile()
    {
        return View();
    }

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
