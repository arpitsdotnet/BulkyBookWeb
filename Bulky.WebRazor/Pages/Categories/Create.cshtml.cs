using Bulky.WebRazor.Data.Base;
using Bulky.WebRazor.Models.Masters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bulky.WebRazor.Pages.Categories;

public class CreateModel : PageModel
{
    [BindProperty]
    public Category Category { get; set; }

    private readonly ApplicationDbContext _dbContext;

    public CreateModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (Category.CategoryName == Category.CategoryDisplayOrder.ToString())
        {
            ModelState.AddModelError(nameof(Category.CategoryName), "Display Order cannot exactly match the Category Name.");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _dbContext.Categories.Add(Category);
        _dbContext.SaveChanges();

        TempData["Success"] = "Category created successfully.";

        return RedirectToPage("Index");
    }
}
