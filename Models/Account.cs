using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models;


public class Account
{   
    // PK Key 
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    // Acc Type
    [Required]
    [Display(Name = "Type")]
    public string AccountType { get; set; }

    // FK customerID
    [Required]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    // Balance
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Balance { get; set; }

   
    [InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }

    public virtual List<BillPay> BillPays { get; set; }


}
