using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;
using Assignment2.Filter;
using Assignment2.Models;
using Assignment2.Utilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using X.PagedList;
using Castle.Core.Resource;
using Newtonsoft.Json;

namespace Assignment2.Controllers;
public class TransactionController : Controller
    {
    private readonly McbaContext _context;

    private const string SessionKey_Account = "TransactionController_Account";

    public TransactionController(McbaContext context) => _context = context;

    public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));

    [HttpPost]
    public async Task<IActionResult> Deposit(int id, decimal amount)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(account);
        }

        LogTransaction(account, amount, "D");

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Customer");
    }

    public async Task<IActionResult> Widthdraw(int id) => View(await _context.Accounts.FindAsync(id));

    [HttpPost]
    public async Task<IActionResult> Widthdraw(int id, decimal amount)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(account);
        }

        LogTransaction(account, -amount, "W");
        if (account.Balance < 0)
        {
            ModelState.AddModelError(nameof(amount), "Insuffcient funds");
            return View(account);
        }
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Customer");
    }


    public async Task<IActionResult> Statement(int id, int? page = 1)
    {
        var Account = await _context.Accounts.FindAsync(id);

        const int pageSize = 4;
        var pagedList = await _context.Transactions.Where(x => x.AccountNumber == Account.AccountNumber).OrderByDescending(x => x.TransactionTimeUtc).ToPagedListAsync(page, pageSize);
        return View(pagedList);
    }

    private void LogTransaction(Account account, decimal amount, string type)
    {   

        account.Balance += amount;

        account.Transactions.Add(
            new Transaction
            {
                TransactionType = type,
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow
            });
        
    }


}
