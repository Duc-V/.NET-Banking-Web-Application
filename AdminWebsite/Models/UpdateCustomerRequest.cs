using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminWebsite.Models;

public class UpdateCustomerRequest
{
    public string Name { get; set; }
    public string TFN { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostCode { get; set; }
    public string Mobile { get; set; }
}

