using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdminWebsite.Models;
using AdminWebsite.Filter;

namespace AdminWebsite.Controllers;
[AuthorizeAdmin]
public class HomeController : Controller
{
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return RedirectToAction("Index", "Customers");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
