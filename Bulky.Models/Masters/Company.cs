using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models.Masters;
public class Company
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [DisplayName("Street address")]
    public string? StreetAddress { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    [DisplayName("Postal code")]
    public int? PostalCode { get; set; }

    [DisplayName("Phone number")]
    public string? PhoneNumber { get; set; }
}
