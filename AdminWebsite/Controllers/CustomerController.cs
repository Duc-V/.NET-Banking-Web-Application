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
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/Customers/{id}");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();

        var customer = JsonConvert.DeserializeObject<Customer>(result);

        return View(customer);
    }


    // POST: Customers/Update/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Customer customer)
    {
        if (id != customer.CustomerID)
            return NotFound();

        if (ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

            var response = Client.PutAsync("api/customers", content).Result;


            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(customer);
    }



    [HttpGet]
    public async Task<IActionResult> Lock(int id)
    {
        Console.WriteLine("1");
        var response = await Client.PutAsync($"api/Customers/{id}/Lock", null);
        Console.WriteLine(response);
        if (!response.IsSuccessStatusCode)
        {
            // Log the error message for debugging purposes
            Console.WriteLine("Error: " + response.StatusCode);
            throw new Exception();
        }
        Console.WriteLine("2");
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Unlock(int id)
    {
        var response = await Client.PutAsync($"api/Customers/{id}/Unlock", null);

        if (!response.IsSuccessStatusCode)
        {
            // Log the error message for debugging purposes
            Console.WriteLine("Error: " + response.StatusCode);
            throw new Exception();
        }

        return RedirectToAction("Index");
    }

}
