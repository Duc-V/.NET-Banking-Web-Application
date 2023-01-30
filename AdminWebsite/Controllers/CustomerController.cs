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
        var response = await Client.GetAsync("api/customers");


        if(!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

        return View(customers);
    }

    
}
