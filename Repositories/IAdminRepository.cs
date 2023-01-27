using AdminAPI.Models;

public interface IAdminRepository
{
    Task<Customer> GetCustomerById(int id);
    Task<bool> UpdateCustomer(int customerId, string name, string tfn, string address, string city, string state, string postcode, string mobile);
    Task<bool> LockCustomer(int id);
    Task<bool> UnlockCustomer(int id);
    Task<List<Customer>> GetAllCustomers();
}
