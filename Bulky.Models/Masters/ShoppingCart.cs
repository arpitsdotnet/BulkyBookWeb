using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.Models.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models.Masters;
public class ShoppingCart
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    [ValidateNever]
    public Product Product { get; set; }

    [Range(1,1000,ErrorMessage = "Please enter a value between 1 and 1000")]
    public int Count { get; set; }

    public string ApplicationUserId { get; set; }
    [ForeignKey(nameof(ApplicationId))]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }

}
