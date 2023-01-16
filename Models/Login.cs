using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models;

public class Login
    
{   

    [Column(TypeName = "char(8)")]
    public string LoginID { get; set; }

    [Required]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }


    [Required]
    [Column(TypeName = "char(94)")]
    public string PasswordHash { get; set; }
}
