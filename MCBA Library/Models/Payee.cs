using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MCBA_Library
{
    public class Payee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PayeeID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Address { get; set; }

        [Required]
        [StringLength(40)]
        public string City { get; set; }

        [Required]
        [StringLength(3)]
        public string State { get; set; }

        [Required]
        [StringLength(4)]
        public string Postcode { get; set; }

        [Required]
        [StringLength(50)]
        public string Phone { get; set; }

        public virtual List<BillPay> BillPay { get; set; }

    }
}
