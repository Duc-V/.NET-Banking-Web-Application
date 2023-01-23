using Assignment2.Data;
using Assignment2.Filter;
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

            await Task.Delay(TimeSpan.FromMinutes(3), cancellationToken);
        }
    }

    private async Task ExecutePaymentAsync(CancellationToken cancellationToken)
    {
         _logger.LogInformation("BillPayment is working.");

        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<McbaContext>();

        var BillPaymetList = await context.BillPay.ToListAsync(cancellationToken);
        var BillPaymentList = BillPaymetList.OrderByDescending(x => x.ScheduleTimeUtc);
        foreach(var BillPay in BillPaymetList)
        {
            if (BillPay.ScheduleTimeUtc > DateTime.UtcNow ) {
                _logger.LogInformation(BillPay.ScheduleTimeUtc.ToString());
                _logger.LogInformation(DateTime.UtcNow.ToString());
            }
        }

        _logger.LogInformation("BillPayment work complete.");
    }
}
