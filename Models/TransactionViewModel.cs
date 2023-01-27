using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models;


public class TransactionViewModel
{   
    public string TransactionType { get; set; }
    public int AccountNumber { get; set; }
    public string AccountType { get; set; }
    [Required]
    public decimal Amount { get; set; }

    public string Comment { get; set; }
    [Required]

    public bool chargeFee { get; set; }
    public int DestinationAccountNumber { get; set; }
}