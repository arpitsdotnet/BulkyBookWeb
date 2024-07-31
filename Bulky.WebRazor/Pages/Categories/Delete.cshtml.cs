using BulkyBook.WebRazor.Data.Base;
using BulkyBook.WebRazor.Models.Masters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyBook.WebRazor.Pages.Categories;

[BindProperties]
public class DeleteModel : PageModel
{
    public Category Category { get; set; }

    private readonly ApplicationDbContext _dbContext;

    public DeleteModel(ApplicationDbContext dbContext)
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
        Category? category = _dbContext.Categories.FirstOrDefault(x => x.CategoryId == Category.CategoryId);
        if (category == null)
            return NotFound();

        _dbContext.Categories.Remove(category);
        _dbContext.SaveChanges();

        TempData["Success"] = "Category deleted successfully.";

        return RedirectToPage("Index");
    }
}
