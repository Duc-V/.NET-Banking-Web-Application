using System.Text;
using Microsoft.AspNetCore.Mvc;
using MCBA_Library;
using Newtonsoft.Json;

namespace AdminWebsite.Controllers;

public class CustomerController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public CustomerController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    // GET: Customers/Index
    public async Task<IActionResult> Index()
    {
        var response = await Client.GetAsync("api/customer");


        if(!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var Customers = JsonConvert.DeserializeObject<List<Customer>>(result);

        return View(Customers);
    }


}
