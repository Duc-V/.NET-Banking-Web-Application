using Assignment2.Data;
using Assignment2.Filter;
using Assignment2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Assignment2.Controllers;
[AuthorizeCustomer]
public class BillPaymentService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BillPaymentService> _logger;

    public BillPaymentService(IServiceProvider services, ILogger<BillPaymentService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        
        while (!cancellationToken.IsCancellationRequested)
        {
            

            _logger.LogInformation("BillPayment service is running");

            await ExecutePaymentAsync(cancellationToken);

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }

    private async Task ExecutePaymentAsync(CancellationToken cancellationToken)
    {
         _logger.LogInformation("BillPayment is working.");

        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<McbaContext>();

        var BillPaymetList = await context.BillPay.ToListAsync(cancellationToken);
        var BillPaymentList = BillPaymetList.Where(x => (x.Status != "Completed") && (x.Status != "Invalid")).OrderByDescending(x => x.ScheduleTimeUtc);

        foreach(var BillPay in BillPaymetList)
        {

            int TimeDifference = (DateTime.UtcNow - BillPay.ScheduleTimeUtc).Minutes;

            // if the time difference between time now and Schedule time is within +-3 minutes, execute
            if ((Enumerable.Range(-3, 3).Contains(TimeDifference)))

            {
                _logger.LogInformation(BillPay.BillPayID.ToString());

                // widthdraw the money from the instructed account number
                var account = context.Accounts.Find(BillPay.AccountNumber);
                // check if the account have sufficient fund
                if (account.AccountType == "Savings" && (account.Balance - BillPay.Amount) <= 0)
                {
                    BillPay.Status = "Invalid";
                    context.SaveChanges();
                    continue;
                }
                else if (account.AccountType == "Checking" && (account.Balance - BillPay.Amount) <= 300)
                {
                    BillPay.Status = "Invalid";
                    context.SaveChanges();
                    continue;
                }
                // if not any of the above case
                account.Balance-= BillPay.Amount;
                

                // log the transaction
                var transaction = new Transaction
                {
                    AccountNumber = BillPay.AccountNumber,
                    TransactionType = "B",
                    Amount = BillPay.Amount,
                    Comment = $"BillPay to PayeeID {BillPay.PayeeID}",
                    TransactionTimeUtc = BillPay.ScheduleTimeUtc.ToLocalTime(),
                };

                _logger.LogInformation("################## Transaction logged");
                _logger.LogInformation(BillPay.BillPayID.ToString());
                context.Transactions.Add(transaction);
                context.SaveChanges();
            }
        }

        _logger.LogInformation("BillPayment work complete.");
    }
}
