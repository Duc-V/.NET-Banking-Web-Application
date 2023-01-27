using System.Threading.Tasks;
using AdminAPI.Models;
using AdminAPI.Repositories;

namespace AdminAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _customerRepository;

        public AdminService(IAdminRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<bool> UpdateCustomer(int id, string name, string address, string city, string state, string postcode, string mobile, string tfn)
        {
            return await _customerRepository.UpdateCustomer(id, name, address, city, state, postcode, mobile, tfn);
        }

        public async Task<bool> LockCustomer(int id)
        {
            return await _customerRepository.LockCustomer(id);
        }

        public async Task<bool> UnlockCustomer(int id)
        {
            return await _customerRepository.UnlockCustomer(id);
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            return await _customerRepository.GetCustomerById(id);
        }
    }
}
