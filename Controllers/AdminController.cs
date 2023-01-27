using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdminAPI.Models;
using AdminAPI.Services;

namespace AdminAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _customerService;

        public AdminController(IAdminService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerById(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer(int id, string name, string tfn, string address, string city, string state, string postcode, string mobile)
        {
            var updated = await _customerService.UpdateCustomer(id, name, tfn, address, city, state, postcode, mobile);

            if (!updated)
                return NotFound();

            return Ok();
        }


        [HttpPut("lock/{id}")]
        public async Task<IActionResult> LockCustomer(int id)
        {
            var customer = await _customerService.LockCustomer(id);

            if (!customer)
                return NotFound();

            return Ok();
        }

        [HttpPut("unlock/{id}")]
        public async Task<IActionResult> UnlockCustomer(int id)
        {
            var customer = await _customerService.UnlockCustomer(id);

            if (!customer)
                return NotFound();

            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLogin model)
        {
            if (model.Username == "admin" && model.Password == "admin")
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }

}
