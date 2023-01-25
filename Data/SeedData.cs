using AdminAPI.Data;
using AdminAPI.Models;
using Newtonsoft.Json;
using System;


namespace AdminAPI.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<McbaContext>();
        // Look for customers.
        if (context.Customers.Any())
            return; // DB has already been seeded.

        // New HTTP connection
        using var client = new HttpClient();
        var json = client.GetStringAsync("https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/").Result;


        //Convert JSON to Customer Object using "Customer" in Models directory
        var Customers = JsonConvert.DeserializeObject<List<Customer>>(json, new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
        });
        // Set account balance to be 0
        decimal AccountBalance = 0;
        // deserialize JSON and uses method above to store JSON data into the data base
        try
        {
            foreach (var Customer in Customers)
            {
                //Add customer
                context.Customers.Add(Customer);
                foreach (var account in Customer.Accounts)
                {
                    if (account.AccountType == "S")
                    {
                        account.AccountType = "Savings";
                    }
                    else
                    {
                        account.AccountType = "Checking";
                    }
                    //reset account balance = 0
                    AccountBalance = 0;
                    foreach (var transaction in account.Transactions)
                    {   // Add transaction
                        AccountBalance += transaction.Amount;
                        transaction.TransactionType = "D";
                        context.Transactions.Add(transaction);

                    }
                    // account balance = sum of transaction
                    account.Balance = AccountBalance;
                    // Add account
                    context.Accounts.Add(account);
                }
                // Add logins
                context.Logins.Add(Customer.Login);
            }
            // Insert dummy Billpay and Payee IDs
            try
            {
                using (StreamReader r = new StreamReader("BillPayDummyData.json"))
                {
                    json = r.ReadToEnd();
                    var Payees = JsonConvert.DeserializeObject<List<Payee>>(json, new JsonSerializerSettings
                    {
                        DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
                    });
                    foreach (var Payee in Payees)
                    {
                        Console.WriteLine(Payee.Name);
                        context.Payee.Add(Payee);
                        foreach (var billPay in Payee.BillPay)
                        {
                            context.BillPay.Add(billPay);
                        }
                    }
                }
            } catch(Exception ex) {
                Console.WriteLine(ex);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        context.SaveChanges();
    }
}
