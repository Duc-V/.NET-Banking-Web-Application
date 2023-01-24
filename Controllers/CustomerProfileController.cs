using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;
using Assignment2.Models;
using SimpleHashing.Net;
using Assignment2.Filter;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
    //[AuthorizeCustomer]
    public async Task<IActionResult> Index()
    {
        // Lazy loading.
        // The Customer.Accounts property will be lazy loaded upon demand.
        var customer = await _context.Customers.FindAsync(CustomerID);
        _customer = customer;
        // OR
        // Eager loading.
        //var customer = await _context.Customers.Include(x => x.Accounts).
        //    FirstOrDefaultAsync(x => x.CustomerID == _customerID);

        return View(customer);
    }



    public async Task<IActionResult> Edit()
    {
        // Lazy loading.
        // The Customer.Accounts property will be lazy loaded upon demand.
        var customer = await _context.Customers.FindAsync(CustomerID);
        _customer = customer;
        // OR
        // Eager loading.
        //var customer = await _context.Customers.Include(x => x.Accounts).
        //    FirstOrDefaultAsync(x => x.CustomerID == _customerID);

        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> SaveChanges([Required, StringLength(50)] string name, [StringLength(50)] string address, [StringLength(40)] string city, [StringLength(4)] string postcode)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var customer = await _context.Customers.FindAsync(CustomerID);
        _customer = customer;

        // Update the customer's information
        _customer.Name = name;
        _customer.Address = address;
        _customer.City = city;
        _customer.PostCode = postcode;
        _context.Customers.Update(_customer);
        await _context.SaveChangesAsync();

        // Redirect the user back to the profile page
        return RedirectToAction("Index", "Customer");
    }

}




