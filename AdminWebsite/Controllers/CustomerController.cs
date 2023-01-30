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




}
