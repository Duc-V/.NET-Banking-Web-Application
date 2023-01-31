using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminWebsite.Models;

public class Customer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int CustomerID { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; }

    [StringLength(11), RegularExpression("^[0-9][0-9][0-9] [0-9][0-9][0-9] [0-9][0-9][0-9]$", ErrorMessage = "Invalid TFN format, Must be of the format:XXX XXX XXX")]
    public string TFN { get; set; }

    [StringLength(50)]
    public string Address { get; set; }

    [StringLength(40)]
    public string City { get; set; }


    [StringLength(3), RegularExpression("^(VIC|NSW|TAS|QLD|SA|WA)$", ErrorMessage = "Invalid state code, Must be of the following: VIC | NSW | TAS | QLD | SA | WA ")]
    public string State { get; set; }

    [StringLength(4)]
    public string PostCode { get; set; }

    [StringLength(12), RegularExpression("^0[4][0-9][0-9] [0-9][0-9][0-9] [0-9][0-9][0-9]$", ErrorMessage = "Invalid mobile number format, Must be of the format: 04XX XXX XXX ")]
    public string Mobile { get; set; }

    public bool IsLocked { get; set; }
}

