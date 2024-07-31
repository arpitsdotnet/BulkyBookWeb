using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyBook.WebRazor.Models.Masters;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [DisplayName("Category Name")]
    [MaxLength(30, ErrorMessage = "Category Name is required.")]
    public string CategoryName { get; set; }

    [DisplayName("Display Order")]
    [Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100.")]
    public int CategoryDisplayOrder { get; set; }
}
