using Assignment2.Data;
using Assignment2.Filter;
using Assignment2.Models;
using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
using System.Diagnostics;

namespace Assignment2.Controllers;
public class HomeController : Controller
{

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    private readonly McbaContext _context;
    private readonly ISimpleHash _simpleHash;

    public HomeController(McbaContext context, ISimpleHash simpleHash)
    {
        _context = context;
        _simpleHash = simpleHash;
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Remove("CustomerID");
        return RedirectToAction("Login");
    }


    // GET request
    public IActionResult Login() => View();


    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        var login = await _context.Logins.FindAsync(loginID);
        if (login == null || string.IsNullOrEmpty(password) || !_simpleHash.Verify(password, login.PasswordHash))
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again. (Password is case sensitive)");
            return View(new Login { LoginID = loginID });
        }

        // Login customer.
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        return RedirectToAction("Index", "Customer");
    }
}
