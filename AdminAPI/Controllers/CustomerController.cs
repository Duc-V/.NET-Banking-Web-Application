using Microsoft.AspNetCore.Mvc;
using AdminAPI.Models;
using AdminAPI.Models.DataManager;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerManager _repo;

    public CustomerController(CustomerManager repo)
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



}
