using System.ComponentModel.DataAnnotations;

namespace Bulky.WebUI.Models.Masters;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    public string CategoryName { get; set; }

    public int CategoryDisplayOrder { get; set; }
}
