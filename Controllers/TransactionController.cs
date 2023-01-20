using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;
using Assignment2.Filter;
using Assignment2.Models;
using Assignment2.Utilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using X.PagedList;
using Castle.Core.Resource;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Controllers;
public class TransactionController : Controller
    {
    private readonly McbaContext _context;

    private const string SessionKey_Account = "TransactionController_Account";

    public TransactionController(McbaContext context) => _context = context;

    public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));

    [HttpPost]
    public async Task<IActionResult> Deposit(int id, decimal amount, string comment = null)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (comment != null && comment.Length > 30)
            ModelState.AddModelError(nameof(comment), "Only 30 characters are allowed");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(account);
        }

        LogTransaction(account, amount, "D", comment);

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


        // check if service fee should be charged
        bool chargeFee = await chargefee(account.AccountNumber);
        decimal serviceFee = 0.05m;
        decimal minimumAmount = 0;
        if (account.AccountType == "Checkings")
        {
            minimumAmount = 300;
        }
        if (chargeFee)
        {
            serviceFee = 0.05m;
            minimumAmount += serviceFee;
            if (amount < minimumAmount)
            {
                ModelState.AddModelError(nameof(amount), "Minimum withdraw amount is " + minimumAmount.ToString());
                return View(account);
            }
        }
        LogTransaction(account, -amount, "W", null);
        if (chargeFee)
        {
            LogTransaction(account, -serviceFee, "S", null);
        }
        if (account.Balance < 0)
        {
            ModelState.AddModelError(nameof(amount), "Insuffcient funds");
            return View(account);
        }
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Customer");
    }


    public async Task<IActionResult> Transfer(int id) => View(await _context.Accounts.FindAsync(id));

    [HttpPost]
    public async Task<IActionResult> Transfer(int id, int destinationId, decimal amount, string comment = null)
    {
        // Find the source and destination accounts using the provided id and destinationId
        var sourceAccount = await _context.Accounts.FindAsync(id);
        var destinationAccount = await _context.Accounts.FindAsync(destinationId);

        // Check if the transfer amount is valid
        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

        // Check if the destination account exists
        if (destinationAccount == null)
            ModelState.AddModelError(nameof(destinationId), "Destination account does not exist.");

        // Check if the destination account is the same as the source account
        if (destinationAccount.AccountNumber == sourceAccount.AccountNumber)
            ModelState.AddModelError(nameof(destinationId), "Destination account cannot be the same as the source account.");
        // Check comment character length 
        if (comment != null && comment.Length > 30)
            ModelState.AddModelError(nameof(comment), "Only 30 characters are allowed");

        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(sourceAccount);
        }

        // Check if a service fee should be charged
        bool chargeFee = await chargefee(sourceAccount.AccountNumber);
        decimal serviceFee = 0.10m;
        decimal minimumAmount = 0;
        if (sourceAccount.AccountType == "Checkings")
        {
            minimumAmount = 300;
        }
        if (chargeFee)
        {
            serviceFee = 0.10m;
            minimumAmount += serviceFee;
            if (amount < minimumAmount)
            {
                ModelState.AddModelError(nameof(amount), "Minimum transfer amount is " + minimumAmount.ToString());
                return View(sourceAccount);
            }
        }
        // Check if the source account has sufficient funds
        if (sourceAccount.Balance < 0)
        {
            ModelState.AddModelError(nameof(amount), "Insufficient funds");
            return View(sourceAccount);
        }
        // Log the transaction for the source account
        LogTransaction(sourceAccount, -amount, "T", comment, _destinationAccountnumber: destinationAccount.AccountNumber);
        if (chargeFee)
        {
            LogTransaction(sourceAccount, -serviceFee, "S", null);
        }



        // Log the transaction for the destination account
        LogTransaction(destinationAccount, amount, "T",comment);

        // Save the changes to the database
        await _context.SaveChangesAsync();

        // Redirect the user to a confirmation page or the customer's account details page
        return RedirectToAction("Index", "Customer");
    }


    public async Task<bool> chargefee(int accountNumber)
    {
        // Count the number of withdrawals for the specified account number
        var withdrawalCount = await _context.Transactions
            .CountAsync(x => x.AccountNumber == accountNumber && x.TransactionType == "W");

        // Count the number of transfers for the specified account number with a non-null destination account number
        var transferCount = await _context.Transactions
            .CountAsync(x => x.AccountNumber == accountNumber && x.TransactionType == "T" && x.DestinationAccountNumber != null);

        // Return true if there are any more than 2 withdrawals or transfers for the specified account number
        return withdrawalCount + transferCount > 2;
    }


    public async Task<IActionResult> Statement(int id, int? page = 1)
    {
        var Account = await _context.Accounts.FindAsync(id);

        ViewBag.Account = Account;
        const int pageSize = 4;
        var pagedList = await _context.Transactions.Where(x => x.AccountNumber == Account.AccountNumber).OrderByDescending(x => x.TransactionTimeUtc).ToPagedListAsync(page, pageSize);
        return View(pagedList);
    }

    private void LogTransaction(Account account, decimal amount, string transactionType, string comment, int _destinationAccountnumber = -1)
    {
        var transaction = new Transaction
        {
            AccountNumber = account.AccountNumber,
            TransactionType = transactionType,
            Amount = amount,
            Comment = comment,
            TransactionTimeUtc = DateTime.UtcNow,

        };
        if (_destinationAccountnumber != -1)
        {
            transaction.DestinationAccount.AccountNumber = _destinationAccountnumber;
        }
        account.Balance += amount;
        _context.Transactions.Add(transaction);
    }






}
