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
        return View(new LoginModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var values = new Dictionary<string, string>
    {
        { "username", model.UserName },
        { "password", model.Password }
    };

        var response = await Client.PostAsync("api/login", new FormUrlEncodedContent(values));

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Customers", "Customer");
        }
        else
        {
            ModelState.AddModelError("", "Incorrect username or password");
            return View("Index", model);
        }
    }


    public async Task<IActionResult> Customer()
    {
        var response = await Client.GetAsync("api/customers");

        if (!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();

        var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

        return View(customers);
    }




}
