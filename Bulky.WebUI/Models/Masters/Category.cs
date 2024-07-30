using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.WebUI.Models.Masters;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [DisplayName("Category Name")]
    public string CategoryName { get; set; }

    [DisplayName("Display Order")]
    public int CategoryDisplayOrder { get; set; }
}
