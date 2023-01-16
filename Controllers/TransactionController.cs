using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;
using Assignment2.Filter;
using Assignment2.Models;
using Assignment2.Utilities;
namespace Assignment2.Controllers;
public class TransactionController : Controller
    {
    private readonly McbaContext _context;

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

        // Note this code could be moved out of the controller, e.g., into the Model.
        account.Balance += amount;
        account.Transactions.Add(
            new Transaction
            {
                TransactionType = "D",
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

}
