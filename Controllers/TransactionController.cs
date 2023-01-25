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
using System.ComponentModel;

namespace Assignment2.Controllers;
[AuthorizeCustomer]
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
        if (comment != null && comment.Length > 30)
            ModelState.AddModelError(nameof(comment), "Only 30 characters are allowed");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(sourceAccount);
        }

        // check if service fee should be charged
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
        }
        if (amount < minimumAmount)
        {
            ModelState.AddModelError(nameof(amount), "Minimum transfer amount is " + minimumAmount.ToString());
            return View(sourceAccount);
        }

        if (destinationAccount != null)
        {
            // Outgoing transfer
            LogTransaction(sourceAccount, -amount, "T", comment, _destinationAccountnumber: destinationAccount.AccountNumber);
            sourceAccount.Balance -= amount;

            // Service fee
            if (chargeFee)
            {
                LogTransaction(sourceAccount, -serviceFee, "S", null, null);
                sourceAccount.Balance -= serviceFee;
            }

            // Incoming transfer
            LogTransaction(destinationAccount, amount, "T", comment);
            destinationAccount.Balance += amount;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Customer");
        }
        else
        {
            ModelState.AddModelError(nameof(destinationId), "Invalid account number");
            return View(sourceAccount);
        }
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
        pagedList = MiscellaneousExtensionUtilities.ConverterUtcTimeToLocalTime(pagedList);
        return View(pagedList);
    }

    private void LogTransaction(Account account, decimal amount, string transactionType, string comment = null, int? _destinationAccountnumber = null)
    {
        var transaction = new Transaction
        {
            Account = account,
            Amount = amount,
            TransactionTimeUtc = DateTime.Now,
            TransactionType = transactionType,
            Comment = comment
        };
        if (_destinationAccountnumber != null)
        {
            transaction.DestinationAccount = _context.Accounts.Find(_destinationAccountnumber);
            transaction.DestinationAccount.AccountNumber = (int)_destinationAccountnumber;
        }
        _context.Transactions.Add(transaction);
        account.Balance += amount;
    }

}
