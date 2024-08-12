using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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


    [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
    public int Count { get; set; }


    public string ApplicationUserId { get; set; }
    [ForeignKey(nameof(ApplicationUserId))]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }


    [NotMapped]
    public double Price => GetPriceBasedOnQuantity();


    public void IncreaseCount(int count)
    {
        Count += count;
        if (Count >= 1000)
            throw new ArgumentOutOfRangeException(nameof(Count));
    }

    public void DecreaseCount(int count)
    {
        Count -= count;
        if (Count <= 0)
            throw new ArgumentOutOfRangeException(nameof(Count));
    }

    public double GetPriceBasedOnQuantity()
    {
        if (Product == null)
            return 0;

        return Count switch
        {
            <= 50 => Product.Price,
            <= 100 => Product.Price50,
            _ => Product.Price100,
        };
    }

    public double GetTotalPriceBasedOnQuantity()
    {
        return (Price * Count);
    }
}
