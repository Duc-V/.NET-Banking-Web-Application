using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Models.Repository;

namespace AdminAPI.Models.DataManager;

public class CustomerManager : ICustomerRepository
{
    private readonly McbaContext _context;

    public CustomerManager(McbaContext context)
    {
        _context = context;
    }


public IEnumerable<Customer> GetAll()
{
    return _context.Customers
        .Select(c => new Customer
        {
            CustomerID = c.CustomerID,
            Name = string.IsNullOrEmpty(c.Name) ? "null" : c.Name
        })
        .ToList();
}

    public Customer Get(int id)
    {
        var customer = _context.Customers
            .Where(c => c.CustomerID == id)
            .Select(c => new Customer
            {
                CustomerID = c.CustomerID,
                Name = c.Name ?? "null",
                Address = c.Address ?? "null",

            })
            .FirstOrDefault();

        return customer;
    }
   
    public void UpdateCustomer(Customer customer, int id)
    {
        var customerToUpdate = _context.Customers.FirstOrDefault(c => c.CustomerID == id);

        if (customerToUpdate != null)
        {
            customerToUpdate.Name = customer.Name;
            customerToUpdate.TFN = customer.TFN;
            customerToUpdate.Address = customer.Address;
            customerToUpdate.City = customer.City;
            customerToUpdate.PostCode = customer.PostCode;
            customerToUpdate.Mobile = customer.Mobile;

            _context.SaveChanges();
        }
    }


}
