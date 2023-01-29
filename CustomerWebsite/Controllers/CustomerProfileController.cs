using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;
using Assignment2.Models;
using SimpleHashing.Net;
using Assignment2.Filter;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using ImageMagick;

namespace Assignment2.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class CustomerProfileController : Controller
{
    private readonly McbaContext _context;
    private readonly ISimpleHash _simpleHash;

    // ReSharper disable once PossibleInvalidOperationException
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerProfileController(McbaContext context, ISimpleHash simpleHash)
    {
        _context = context;
        _simpleHash = simpleHash;
    }

    private Customer _customer;
    private Login _login;
    //[AuthorizeCustomer]
    public async Task<IActionResult> Index()
    {
        // Lazy loading.
        // The Customer.Accounts property will be lazy loaded upon demand.
        _customer = await _context.Customers.FindAsync(CustomerID); ;
        // OR
        // Eager loading.
        //var customer = await _context.Customers.Include(x => x.Accounts).
        //    FirstOrDefaultAsync(x => x.CustomerID == _customerID);


        var viewModel = new ViewModel
        {
            Customer = _customer
        };

        return View(viewModel);
    }



    public async Task<IActionResult> Edit()
    {
        // Lazy loading.
        // The Customer.Accounts property will be lazy loaded upon demand.
        _customer = await _context.Customers.FindAsync(CustomerID); ;
        // OR
        // Eager loading.
        //var customer = await _context.Customers.Include(x => x.Accounts).
        //    FirstOrDefaultAsync(x => x.CustomerID == _customerID);


        var viewModel = new ViewModel
        {
            Customer = _customer
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> SaveProfile(ViewModel model)
    {
        _customer = await _context.Customers.FindAsync(CustomerID);
        
        var viewModel = new ViewModel();
        
        var customer = model.Customer;
        var file = HttpContext.Request.Form.Files["Customer.ProfilePicture"];
        if (file != null && file.Length > 0)
        {
            using (MagickImage image = new MagickImage(file.OpenReadStream()))
            {
                if (image.Width > image.Height)
                {
                    image.Resize(400, 0);
                } else if(image.Height > image.Width)
                {
                    image.Resize(0, 400);
                } else
                {
                    // Equal
                    image.Resize(400, 400);
                }

                image.Format = MagickFormat.Jpeg;
                customer.ProfilePicture = image.ToByteArray();
            }
        }

        if (!ModelState.IsValid)
        {
            viewModel = new ViewModel
            
            {
                Customer = _customer,
                Errors = new string[] {"Some attributes don't follow the validation rules"}
            };

            return View("Edit", viewModel);
        }

        // Update the customer's information
        _customer.ProfilePicture = model.Customer.ProfilePicture;
        _customer.Name = model.Customer.Name;
        _customer.Address = model.Customer.Address;
        _customer.City = model.Customer.City;
        _customer.PostCode = model.Customer.PostCode;
        _context.Customers.Update(_customer);
        
        HttpContext.Session.SetString(nameof(Customer.Name), model.Customer.Name);
        
        await _context.SaveChangesAsync();
        
        viewModel = new ViewModel
        {
            Customer = _customer,
            Successes = new string[] { "Profile updated successfully" }
        };

        // Redirect the user back to the profile page
        return View("Index", viewModel);
    }


    public IActionResult UpdatePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SavePassword([Required] string oldPassword, [Required] string newPassword, [Required] string confirmNewPassword)
    {
        var viewModel = new ViewModel();
        if (!ModelState.IsValid)
        {
            viewModel = new ViewModel
            {
                Errors = new string[] { "All fields are required; Validation rules not met." }
            };

            return View("UpdatePassword", viewModel);
        }

        _customer = await _context.Customers.FindAsync(CustomerID);
        _login = await _context.Logins.SingleOrDefaultAsync(login => login.CustomerID == _customer.CustomerID);

        // Check if the old password is correct
        if (!_simpleHash.Verify(oldPassword, _login.PasswordHash))
        {
            viewModel = new ViewModel
            {
                Errors = new string[] { "Old password doesn't match our records." }
            };
            return View("UpdatePassword", viewModel);
        }
        if (newPassword != confirmNewPassword)
        {
            viewModel = new ViewModel
            {
                Errors = new string[] { "The new passwords don't match..." }
            };

            return View("UpdatePassword", viewModel);
        }

        // Update the password

        _login.PasswordHash = _simpleHash.Compute(newPassword);
        _context.Logins.Update(_login);
        await _context.SaveChangesAsync();

        viewModel = new ViewModel
        {
            Customer = _customer,
            Successes = new string[] { "Password changed successfully." }
        };
        return View("Index", viewModel);
    }
}




