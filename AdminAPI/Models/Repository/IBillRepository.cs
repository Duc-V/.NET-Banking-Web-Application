namespace AdminAPI.Models.Repository;

public interface IBillRepository
{
    // return all accounts
    IEnumerable<Account> GetAccounts(int id);

    // get billpay from account number 
    IEnumerable<BillPay> GetBillPayTransactions(int accountNumber);

    // block bill pay
    void Block(int id);


    // unblock bill pay
    void Unblock(int id);

}


