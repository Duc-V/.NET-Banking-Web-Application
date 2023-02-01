using Microsoft.AspNetCore.Mvc;
using AdminAPI.Models;
using AdminAPI.Models.DataManager;
using Microsoft.EntityFrameworkCore;

namespace AdminAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillPayController : ControllerBase
{
    private readonly BillManager _repo;

    public BillPayController(BillManager repo)
    {
        _repo = repo;
    }

    // GET: api/BillPay ---> get all accounts and display for customer id
    [HttpGet]
    public IEnumerable<Account> Get(int id)
    {
        return _repo.GetAccounts(id);
    }


    // once account is selected get all bill pay transactions using account number
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