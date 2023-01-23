using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Assignment2.Controllers
{
    public class BillPayController : Controller
    {
        private readonly McbaContext _context;
        private Account account = new Account();

        public BillPayController(McbaContext context) => _context = context;


        public IActionResult Index(int id)
        {
            account = _context.Accounts.Find(id);
            ViewBag.Id = id;
            return View(_context.BillPay.Where(x => x.AccountNumber == id).ToList());
        }

        public IActionResult AddNewBillPay() => View();

        [HttpPost]
        public async Task<IActionResult> AddNewBillPay(int PayeeID, decimal Amount, DateTime DateTime, string Period)
        {
            Console.WriteLine(account.Balance);
            if (account.AccountType == "Savings" && account.Balance <= 0)
                ModelState.AddModelError("InvalidFunds", "Invalid fund");

            if (account.AccountType == "Checking" && (account.Balance - Amount) <= 300)
                ModelState.AddModelError("InvalidFunds", "Invalid fund");

            if (!_context.Payee.Any(x => x.PayeeID == PayeeID))
                ModelState.AddModelError(nameof(PayeeID), "PayeeID doesn't exist");
            if (Amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(Amount), "Amount cannot have more than 2 decimal places.");
            if (DateTime.Now > DateTime) 
                ModelState.AddModelError(nameof(DateTime), "Cannot schedule for time in the past");
            
            if (!ModelState.IsValid)
                return View();
            
            return View(account);
        }
    }
}
