using Microsoft.AspNetCore.Mvc;
using AdminAPI.Models;
using AdminAPI.Models.DataManager;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillPayController : ControllerBase
{
    private readonly BillPayManager _repo;

    public BillPayController(BillPayManager repo)
    {
        _repo = repo;
    }

    // GET: api/BillPay ---> get all accounts and display for customer id
    [HttpGet("{id}")]
    public IEnumerable<Account> Get(int id)
    {
        return _repo.GetAccounts(id);
    }

    
    // once account is selected get all bill pay transactions using account number - works!
    [HttpGet("{accountNumber}/transactions")]
    public IEnumerable<BillPay> GetBillPayTransactions(int accountNumber)
    {
        return _repo.GetBillPayTransactions(accountNumber);
    }




    // change billpay status blocked / unblocked


    [HttpPut("{id}/block")]
    public void Block(int id)
    {
        _repo.Block(id);
    }

    [HttpPut("{id}/unblock")]
    public void Unblock(int id)
    {
        _repo.Unblock(id);
    }






}