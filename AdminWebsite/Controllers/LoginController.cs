using Microsoft.AspNetCore.Mvc;

namespace AdminWebsite.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index() => View();


        [HttpPost]
        public IActionResult Login(string loginID, string password)
        {
            if (loginID != "admin" || password != "admin")
            {
                return View("Login/Index");
            }

            // Login customer.
            HttpContext.Session.SetString("id", "admin");
            return RedirectToAction("Index", "Customer");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
