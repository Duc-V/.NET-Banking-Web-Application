using AdminAPI.Data;
using AdminAPI.Models;
using AdminAPI.Models.Repository;

namespace AdminAPI.Models.DataManager;

public class BillPayManager : IBillRepository
{
    private readonly McbaContext _context;

    public BillPayManager(McbaContext context)
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
