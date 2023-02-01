using System.Text;
using Microsoft.AspNetCore.Mvc;
using AdminWebsite.Models;
using Newtonsoft.Json;
namespace AdminWebsite.Controllers
{
    public class BillPayController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");

        public BillPayController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

        public async Task<IActionResult> Index(int id)
        {
            
        }



    }
}