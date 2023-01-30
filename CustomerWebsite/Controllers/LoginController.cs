using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
using Assignment2.Data;
using Assignment2.Models;

namespace Assignment2.Controllers;

// Bonus Material: Implement global authorisation check.
//[AllowAnonymous]
//[Route("/Mcba/SecureLogin")]
public class LoginController : Controller
{
    private readonly McbaContext _context;
    private readonly ISimpleHash _simpleHash;
    public LoginController(McbaContext context, ISimpleHash simpleHash)
    {
        _context = context;
        _simpleHash = simpleHash;
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

    [Route("LogoutNow")]
    public IActionResult Logout()
    {
        // Logout customer.
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}
