using Assignment2.Data;
using Microsoft.AspNetCore.Mvc;
namespace Assignment2.Controllers
{
    public class BillPayController : Controller
    {
        private readonly McbaContext _context;

        public BillPayController(McbaContext context) => _context = context;

        public async Task<IActionResult> BillPay()
        {
            var test = _context.Payee.ToList();
            foreach(var item in test)
                Console.WriteLine(item);
            return Ok();
        }
    }
}
