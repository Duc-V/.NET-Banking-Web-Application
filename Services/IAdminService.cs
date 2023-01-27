using System.Threading.Tasks;
using AdminAPI.Models;

namespace AdminAPI.Services
{
    public interface IAdminService
    {
        Task<bool> UpdateCustomer(int id, string name, string address, string city, string state, string postcode, string mobile, string tfn);
        Task<bool> LockCustomer(int id);
        Task<bool> UnlockCustomer(int id);
        Task<Customer> GetCustomerById(int id);
        Task<List<Customer>> GetAllCustomers();
    }
}
