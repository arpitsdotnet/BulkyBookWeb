using Bulky.WebRazor.Data.Base;
using Bulky.WebRazor.Models.Masters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bulky.WebRazor.Pages.Categories;

public class IndexModel : PageModel
{
    public List<Category> CategoryList { get; set; }

    private readonly ApplicationDbContext _dbContext;

    public IndexModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void OnGet()
    {
        CategoryList = _dbContext.Categories.ToList();
    }
}
