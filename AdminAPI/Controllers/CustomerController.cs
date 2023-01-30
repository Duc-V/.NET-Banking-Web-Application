using Microsoft.AspNetCore.Mvc;
using AdminAPI.Models;
using AdminAPI.Models.DataManager;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerManager _repo;

    public CustomersController(CustomerManager repo)
    { 
        _repo = repo; 
    }

    // GET: api/customers
    [HttpGet]
    public IEnumerable<Customer> Get()
    {
        return _repo.GetAll();
    }



    // GET: api/customers/{id}
    [HttpGet("{id}")]
    public ActionResult<Customer> Get(int id)
    {
        var customer = _repo.Get(id);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }


    [HttpPut("{id}")]
    public IActionResult UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request)
    {
        var customer = _repo.Get(id);
        if (customer == null)
        {
            return NotFound();
        }

        customer.Name = request.Name;
        customer.TFN = request.TFN;
        customer.Address = request.Address;
        customer.City = request.City;
        customer.State = request.State;
        customer.PostCode = request.PostCode;
        customer.Mobile = request.Mobile;

        _repo.UpdateCustomer(customer, id);

        return NoContent();
    }








}
