using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    public class BillPay
    {   
        // BillPay
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BillPayID { get; set; }

        // AccountNumber FK
        [Required]
        [ForeignKey("Account")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        // PayeeID
        [Required]
        public int PayeeID{ get; set; }

        // Amount
        [Column(TypeName = "money")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        
        // Date
        [DataType(DataType.DateTime)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ScheduleTimeUtc { get; set; }

        // Period
        [Required]
        [Column(TypeName = "char(1)")]
        public string Period { get; set; }

    }
}
