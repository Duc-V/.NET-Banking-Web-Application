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
using System;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Assignment2.Controllers;
[AuthorizeCustomer]
public class TransactionController : Controller
    {
    private readonly McbaContext _context;

    private const string SessionKey_Account = "TransactionController_Account";

    public TransactionController(McbaContext context) => _context = context;

    public IActionResult Deposit(int id)
    {
        var _CustomerID = HttpContext.Session.GetInt32("CustomerID");
        var account = _context.Accounts.FirstOrDefault(x => (x.CustomerID == _CustomerID) && (x.AccountNumber == id));
        if (account == null)
        {
            return RedirectToAction("Menu", "Customer", new { id = id });
        }

        var ViewModel = new TransactionViewModel
        {
            AccountNumber = account.AccountNumber,
            AccountType = account.AccountType,
        };
        return View(ViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Deposit(TransactionViewModel ViewModel)
    {
        ModelState.Clear();
        var account = await _context.Accounts.FindAsync(ViewModel.AccountNumber);

        if (ViewModel.Amount <= 0)
            ModelState.AddModelError(nameof(ViewModel.Amount), "Amount must be positive.");
        if (ViewModel.Amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(ViewModel.Amount), "Amount cannot have more than 2 decimal places.");
        if (ViewModel.Comment != null && ViewModel.Comment.Length > 30)
            ModelState.AddModelError(nameof(ViewModel.Comment), "Only 30 characters are allowed");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = ViewModel.Amount;
            return View(ViewModel);
        }

        ViewModel.TransactionType = "Deposit";

        return RedirectToAction("ConfirmTransaction", ViewModel);
    }



    public IActionResult Widthdraw(int id) {
        var _CustomerID = HttpContext.Session.GetInt32("CustomerID");
        var account = _context.Accounts.FirstOrDefault(x => ( x.CustomerID == _CustomerID ) && (x.AccountNumber == id));
        if (account == null)
        {
            return RedirectToAction("Menu", "Customer", new {id = id});
        }

        var ViewModel = new TransactionViewModel
        {
            AccountNumber = account.AccountNumber,
            AccountType = account.AccountType,
        };
        return View(ViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Widthdraw(TransactionViewModel ViewModel)
    {
        ModelState.Clear();
        var account = await _context.Accounts.FindAsync(ViewModel.AccountNumber);
        bool chargeFee = await chargefee(account.AccountNumber);
        decimal serviceFee = 0.05m;
        decimal minimumAmount = 0;
        

        if (ViewModel.Amount <= 0)
            ModelState.AddModelError(nameof(ViewModel.Amount), "Amount must be positive.");
        if (ViewModel.Amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(ViewModel.Amount), "Amount cannot have more than 2 decimal places.");

        // check if service fee should be charged

        if (account.AccountType == "Checkings")
            minimumAmount = 300;
        if (chargeFee)
        {
            minimumAmount += serviceFee;
            if ((account.Balance - ViewModel.Amount) < minimumAmount)
            {
                ModelState.AddModelError(nameof(ViewModel.Amount), "Insuffcient funds");
            }
        }

        if ((account.Balance - ViewModel.Amount) < 0)
            ModelState.AddModelError(nameof(ViewModel.Amount), "Insuffcient funds");


        if (!ModelState.IsValid)
        {
            ViewBag.Amount = ViewModel.Amount;
            return View(ViewModel);
        }
        //set chargeFee to ViewModel
        ViewModel.chargeFee = chargeFee;
        //set transaction typ to ViewModel
        ViewModel.TransactionType = "Widthdraw";

        return RedirectToAction("ConfirmTransaction", ViewModel);
    }


    public IActionResult ConfirmTransaction(TransactionViewModel ViewModel)
    {
        return View(ViewModel);
    } 

    [HttpPost]
    public async Task<IActionResult> ConfirmTransactionPost(TransactionViewModel ViewModel) {

        var account = await _context.Accounts.FindAsync(ViewModel.AccountNumber);

        if (ViewModel.TransactionType == "Widthdraw")
        {
            LogTransaction(account, -ViewModel.Amount, "W", null);
            if (ViewModel.chargeFee)
            {
                LogTransaction(account, -0.05m, "S", null);
            }
        }
        else if (ViewModel.TransactionType == "Deposit")
        {

            LogTransaction(account, ViewModel.Amount, "D", ViewModel.Comment);

            
        }
        else if (ViewModel.TransactionType == "BillPay")
        {
            var BillPay = new BillPay
            {
                AccountNumber = ViewModel.AccountNumber,
                PayeeID = ViewModel.DestinationAccountNumber,
                Amount = ViewModel.Amount,
                ScheduleTimeUtc = TimeZoneInfo.ConvertTimeToUtc(ViewModel.DateTime, TimeZoneInfo.Local),
                Period = ViewModel.Period
            };
            _context.BillPay.Add(BillPay);
        }
        else if(ViewModel.TransactionType == "Transfer")
        {
            var sourceAccount = await _context.Accounts.FindAsync(ViewModel.AccountNumber);
            var destinationAccount = await _context.Accounts.FindAsync(ViewModel.DestinationAccountNumber);

            // Outgoing transfer
            LogTransaction(sourceAccount, -ViewModel.Amount, "T", ViewModel.Comment, ViewModel.DestinationAccountNumber);

            // Service fee
            if (ViewModel.chargeFee)
            {
                LogTransaction(sourceAccount, -0.10m, "S", null, null);
            }

            // Incoming transfer
            LogTransaction(destinationAccount, ViewModel.Amount, "T", ViewModel.Comment);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Customer");
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Menu", "Customer", new { id = ViewModel.AccountNumber });

    }


    public IActionResult Transfer(int id)
    {
        var _CustomerID = HttpContext.Session.GetInt32("CustomerID");
        var account = _context.Accounts.FirstOrDefault(x => (x.CustomerID == _CustomerID) && (x.AccountNumber == id));
        if (account == null)
        {
            return RedirectToAction("Menu", "Customer", new { id = id });
        }

        var ViewModel = new TransactionViewModel
        {
            AccountType = account.AccountType, 
            AccountNumber = account.AccountNumber,

        };
        return View(ViewModel);
    
    }

    [HttpPost]
    public async Task<IActionResult> Transfer(TransactionViewModel ViewModel)
    {

        ModelState.Clear();

        // Find the source and destination accounts using the provided id and destinationId
        var sourceAccount = await _context.Accounts.FindAsync(ViewModel.AccountNumber);
        var destinationAccount = await _context.Accounts.FindAsync(ViewModel.DestinationAccountNumber);




        //Check if the transfer amount is valid
        if (ViewModel.DestinationAccountNumber == ViewModel.AccountNumber)
            ModelState.AddModelError("DestinationAccountNumber", "Can't transfer to the same account");
        if ((sourceAccount.Balance - ViewModel.Amount) < 0)
            ModelState.AddModelError("Amount", "Not Enough Funds");

        if (ViewModel.Amount <= 0)
            ModelState.AddModelError("Amount", "Amount must be positive.");

        if (ViewModel.Amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError("Amount", "Amount cannot have more than 2 decimal places.");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = ViewModel.Amount;
            return View(ViewModel);
        }



        // check if service fee should be charged
        bool chargeFee = await chargefee(sourceAccount.AccountNumber);
        decimal serviceFee = 0.10m;
        decimal minimumAmount = 0;
        if (sourceAccount.AccountType == "Checking")
        {
            minimumAmount = 300;
        }
        if (chargeFee)
        {
            serviceFee = 0.10m;
            minimumAmount += serviceFee;
        }
        if ((sourceAccount.Balance - ViewModel.Amount) < minimumAmount)
        {
            ModelState.AddModelError("Amount", $"Not Enough Funds For {sourceAccount.AccountType} Account");
            return View(ViewModel);
        }

        if (destinationAccount != null)
        {
            ViewModel.chargeFee = chargeFee;
            ViewModel.TransactionType = "Transfer";

            return RedirectToAction("ConfirmTransaction", ViewModel);
        }
        else
        {
            ModelState.AddModelError("DestinationAccountNumber", "Invalid account number");
            return View(ViewModel);
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
        Console.WriteLine("####################################");
        Console.WriteLine(withdrawalCount + transferCount);
        return (withdrawalCount + transferCount) >= 2;
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
            TransactionTimeUtc = DateTime.UtcNow,
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
