using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BulkyBook.Models.Masters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models.Identity;
public class ApplicationUser : IdentityUser
{
    [Required]
    public string Name { get; set; }

    public string? StreetAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }

    public int? CompanyId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    [ValidateNever]
    public Company? Company { get; set; }

    [NotMapped]
    public string Role { get; set; }

}
