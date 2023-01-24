using Assignment2.Data;
using Assignment2.Filter;
using Assignment2.Models;
using Assignment2.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Assignment2.Controllers
{
    [AuthorizeCustomer]
    public class BillPayController : Controller
    {
        private readonly McbaContext _context;
        private int AccountNumber => HttpContext.Session.GetInt32("AccountNumber").Value;

        public BillPayController(McbaContext context) => _context = context;


        public IActionResult Index(int id)
        {
            ViewBag.Id = id;
            return View(_context.BillPay.Where(x => x.AccountNumber == id).ToList());
        }

        public IActionResult AddNewBillPay() => View();

        [HttpPost]
        public async Task<IActionResult> AddNewBillPay(int PayeeID, decimal Amount, DateTime DateTime, string Period)
        {
            var account = _context.Accounts.Find(AccountNumber);
            Console.WriteLine(account.Balance);
            if (account.AccountType == "Savings" && (account.Balance - Amount) <= 0)
                ModelState.AddModelError("InvalidFunds", "Not Enough Funds");

            if (account.AccountType == "Checking" && (account.Balance - Amount) <= 300)
                ModelState.AddModelError("InvalidFunds", "Funds cannot be under $300");

            if (!_context.Payee.Any(x => x.PayeeID == PayeeID))
                ModelState.AddModelError(nameof(PayeeID), "PayeeID doesn't exist");
            if (Amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(Amount), "Amount cannot have more than 2 decimal places.");
            if (DateTime.Now > DateTime) 
                ModelState.AddModelError(nameof(DateTime), "Cannot schedule for time in the past");
            
            if (!ModelState.IsValid)
                return View();

            // new bill pay account object
            var BillPay = new BillPay
            {
                AccountNumber = account.AccountNumber,
                PayeeID = PayeeID,
                Amount = Amount,
                ScheduleTimeUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo.Local),
                Period = Period
            };
            _context.BillPay.Add(BillPay);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "BillPay", new { id = AccountNumber});
        }
    }
}
