using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models;


public class TransactionViewModel
{   
    
    public int AccountNumber { get; set; }

    public int DestinationAccountNumber { get; set; }
    public string TransactionType { get; set; }
    public string AccountType { get; set; }
    public decimal Amount { get; set; }
   
    public DateTime DateTime { get; set; }

    public string Period { get; set; }
    public string Comment { get; set; }

    public bool chargeFee { get; set; }

    
}