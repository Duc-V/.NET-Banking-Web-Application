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
        var list = BillPaymetList.Where(x => (x.Status != "Complete") && (x.Status != "Not Enough Funds At Scheduled Time") && (x.Status != "Cancelled") && (x.Status != "Blocked")).OrderByDescending(x => x.ScheduleTimeUtc);

        foreach(var BillPay in list)
        {
            // check the time difference between billpay time and current time in minutes 
            int TimeDifference = Convert.ToInt32((DateTime.UtcNow - BillPay.ScheduleTimeUtc).TotalMinutes);
            // if the time difference between time now and Schedule time is within +-3 minutes OR transactions not executed in the past, execute
            if ((Enumerable.Range(-3, 3).Contains(TimeDifference)) || (BillPay.ScheduleTimeUtc < DateTime.UtcNow))

            {

                // widthdraw the money from the instructed account number
                var account = context.Accounts.Find(BillPay.AccountNumber);
                // check if the account have sufficient fund
                if (account.AccountType == "Savings" && (account.Balance - BillPay.Amount) <= 0)
                {
                    BillPay.Status = "Not Enough Funds At Scheduled Time";
                    context.SaveChanges();
                    continue;
                }
                else if (account.AccountType == "Checking" && (account.Balance - BillPay.Amount) <= 300)
                {
                    BillPay.Status = "Not Enough Funds At Scheduled Time";
                    context.SaveChanges();
                    continue;
                }
                // if not any of the above cases
                account.Balance -= BillPay.Amount;


                // log the transaction
                var transaction = new Transaction
                {
                    AccountNumber = BillPay.AccountNumber,
                    TransactionType = "B",
                    Amount = - BillPay.Amount,
                    Comment = $"BillPay to PayeeID {BillPay.PayeeID}",
                    TransactionTimeUtc = BillPay.ScheduleTimeUtc,
                };
                context.Transactions.Add(transaction);


                // change the bill pay status to Complete
                BillPay.Status = "Complete";

                // if Period == One-Off, schedule the next billpay
                if (BillPay.Period == "Monthly")
                {
                    var NextMonthBpay = new BillPay
                    {
                        AccountNumber = BillPay.AccountNumber,
                        PayeeID = BillPay.PayeeID,
                        Amount = BillPay.Amount,
                        ScheduleTimeUtc = BillPay.ScheduleTimeUtc.AddMonths(1),
                        Period = BillPay.Period
                    };

                    context.BillPay.Add(NextMonthBpay);
                }
                _logger.LogInformation($"################## Transaction No.{BillPay.BillPayID} logged #################");



                context.SaveChanges();
            }
        }

        _logger.LogInformation("BillPayment work complete.");
    }

}
