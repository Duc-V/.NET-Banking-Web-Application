using System.Text;
using Microsoft.AspNetCore.Mvc;
using AdminWebsite.Models;
using Newtonsoft.Json;

namespace AdminWebsite.Controllers;

public class CustomersController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public CustomersController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    // GET: Customers/Index




    public async Task<IActionResult> Index()
    {
        var response = await Client.GetAsync("api/Customers");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();

        var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

        return View(customers);
    }

    // GET: Customers/Update/{id}
    public async Task<IActionResult> Update(int id)
    {
        var response = await Client.GetAsync($"api/Customers/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();

        var customer = JsonConvert.DeserializeObject<Customer>(result);

        return View(customer);
    }


    // POST: Customers/Update/{id}
    [HttpPut]
    public async Task<IActionResult> Update(int id, UpdateCustomerRequest request)
    {
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await Client.PutAsync($"api/Customers/{id}", content);

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        return RedirectToAction("Index");
    }


}
