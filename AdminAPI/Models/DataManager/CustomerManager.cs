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
    return _context.Customers.ToList();
}

    public Customer Get(int id)
    {
        var customer = _context.Customers.Where(c => c.CustomerID == id).FirstOrDefault();
        return customer;
    }
    public int Update(int id, Customer customer)
    {
        _context.Update(customer);
        _context.SaveChanges();

        return id;
    }



}
