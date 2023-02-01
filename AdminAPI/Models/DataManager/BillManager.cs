using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Models.Repository;

namespace AdminAPI.Models.DataManager;

public class BillManager
{
    private readonly McbaContext _context;

    public BillManager(McbaContext context)
    {
        _context = context;
    }


    // return list of accounts for given customer id
    public IEnumerable<Account> GetAccounts(int id)
    {
      return _context.Accounts.Where(a => a.CustomerID == id).ToList();
    }

    // return bill pay transactions for given account number
    public IEnumerable<BillPay> GetBillPayTransactions(int accountNumber)
    {
        return _context.BillPay.Where(b => b.Account.AccountNumber == accountNumber).ToList();
    }



    public Customer Get(int id)
    {
        var customer = _context.Customers
            .Where(c => c.CustomerID == id)
            .FirstOrDefault();

        return customer;
    }
    public int Update(int id, Customer customer)
    {
        _context.Update(customer);
        _context.SaveChanges();

        return id;
    }

    public void Block(int id)
    {
        var billPay = _context.BillPay.Find(id);
        if (billPay != null)
        {
            billPay.Status = billPay.Status == "blocked" ? billPay.Status : "blocked";
            _context.SaveChanges();
        }
    }

    public void Unblock(int id)
    {
        var billPay = _context.BillPay.Find(id);
        if (billPay != null)
        {
            billPay.Status = billPay.Status == "" ? billPay.Status : "";
            _context.SaveChanges();
        }
    }



}
