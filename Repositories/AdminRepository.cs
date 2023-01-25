using AdminAPI.Data;
using AdminAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly McbaContext _context;

    public AdminRepository(McbaContext context)
    {
        _context = context;
    }

    public async Task<Customer> GetCustomerById(int id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<bool> UpdateCustomer(int customerId, string name, string tfn, string address, string city, string state, string postcode, string mobile)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerID == customerId);
        if (customer == null)
        {
            return false;
        }

        customer.Name = name;
        customer.TFN = tfn;
        customer.Address = address;
        customer.City = city;
        customer.State = state;
        customer.PostCode = postcode;
        customer.Mobile = mobile;

        _context.Customers.Update(customer);
        return await _context.SaveChangesAsync() > 0;
    }


    public async Task<bool> LockCustomer(int id)
    {
        var customer = await GetCustomerById(id);
        if (customer == null)
        {
            return false;
        }
        customer.IsLocked = true;
        _context.Customers.Update(customer);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UnlockCustomer(int id)
    {
        var customer = await GetCustomerById(id);
        if (customer == null)
        {
            return false;
        }
        customer.IsLocked = false;
        _context.Customers.Update(customer);
        return await _context.SaveChangesAsync() > 0;
    }




}
