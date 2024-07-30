using Bulky.WebRazor.Data.Base;
using Bulky.WebRazor.Models.Masters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bulky.WebRazor.Pages.Categories;

[BindProperties]
public class EditModel : PageModel
{
    public Category Category { get; set; }

    private readonly ApplicationDbContext _dbContext;

    public EditModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult OnGet(int? CategoryId)
    {
        if (CategoryId is null or 0)
            return NotFound();

        //Category? category = _dbContext.Categories.Find(categoryId);
        Category = _dbContext.Categories.FirstOrDefault(x => x.CategoryId == CategoryId);

        if (Category is null)
            return NotFound();

        return Page();
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

        _dbContext.Categories.Update(Category);
        _dbContext.SaveChanges();

        TempData["Success"] = "Category updated successfully.";

        return RedirectToPage("Index");
    }
}
