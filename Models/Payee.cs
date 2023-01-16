using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment2.Models
{
    public class Payee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PayeeID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Address { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(40)")]
        public string City { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(3)")]
        public string State { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(4)")]
        public string Postcode { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Phone { get; set; }

        public virtual List<BillPay> BillPays { get; set; }

    }
}
