using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;
using Assignment2.Models;

namespace Assignment2.Controllers
{
    public class CustomerProfileController : Controller
    {
        private readonly McbaContext _context;

        public CustomerProfileController(McbaContext context) => _context = context;

        // GET request to view customer profile
        public async Task<IActionResult> Index()
        {
            // Verify that the user is logged in
            if (!HttpContext.Session.TryGetValue(nameof(Customer.CustomerID), out var customerIDValue))
            {
                return RedirectToAction("Login", "Login");
            }

            // Get the customer's information
            var customerID = BitConverter.ToInt32(customerIDValue);
            var customer = await _context.Customers.FindAsync(customerID);

            // Display the customer's information in a view
            return View(customer);
        }

        // POST request to save changes to customer information
        [HttpPost]
        public async Task<IActionResult> SaveChanges(Customer updatedCustomer)
        {
            // Verify that the user is logged in
            if (!HttpContext.Session.TryGetValue(nameof(Customer.CustomerID), out var customerIDValue))
            {
                return RedirectToAction("Login", "Login");
            }

            // Get the current customer information
            var customerID = BitConverter.ToInt32(customerIDValue);
            var customer = await _context.Customers.FindAsync(customerID);

            // Update the customer's information
            customer.Name = updatedCustomer.Name;
            customer.Address = updatedCustomer.Address;
            customer.City = updatedCustomer.City;
            customer.PostCode = updatedCustomer.PostCode;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            // Redirect the user back to the profile page
            return RedirectToAction("Index");
        }

        // GET request to view change password form
        public IActionResult ChangePassword()
        {
            // Verify that the user is logged in
            if (!HttpContext.Session.TryGetValue(nameof(Customer.CustomerID), out var customerIDValue))
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }

        // POST request to change the customer's password
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmNewPassword)
        {
            // Verify that the user is logged in
            if (!HttpContext.Session.TryGetValue(nameof(Customer.CustomerID), out var customerIDValue))
            {
                return RedirectToAction("Login", "Login");
            }

            // Get the current customer information
            var customerID = BitConverter.ToInt32(customerIDValue);
            var customer = await _context.Customers.FindAsync(customerID);

            // Verify that the user is logged in
            if (!HttpContext.Session.TryGetValue(nameof(Customer.CustomerID), out var customerIDValue))
            {
                return RedirectToAction("Login", "Login");
            }

            // Get the current customer information
            var customerID = BitConverter.ToInt32(customerIDValue);
            var customer = await _context.Customers.FindAsync(customerID);

            // Verify that the current password is correct
            if (!s_simpleHash.Verify(currentPassword, customer.Login.PasswordHash))
            {
                ModelState.AddModelError("CurrentPasswordIncorrect", "The current password is incorrect, please try again.");
                return View();
            }// Verify that the new password and confirmation match
            if (newPassword != confirmNewPassword)
            {
                ModelState.AddModelError("NewPasswordMismatch", "The new password and confirmation do not match, please try again.");
                return View();
            }

            // Hash the new password
            var newPasswordHash = s_simpleHash.Compute(newPassword);
            customer.Login.PasswordHash = newPasswordHash;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            // Redirect the user back to the profile page
            return RedirectToAction("Index");
        }
    }
    }
