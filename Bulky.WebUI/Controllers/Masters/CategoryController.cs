using Bulky.WebUI.Data.Base;
using Bulky.WebUI.Models.Masters;
using Microsoft.AspNetCore.Mvc;

namespace Bulky.WebUI.Controllers.Masters;
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryController(
        ApplicationDbContext dbContext
        )
    {
        _dbContext = dbContext;
    }
    public IActionResult Index()
    {
        List<Category> categories = _dbContext.Categories.ToList();

        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (category.CategoryName == category.CategoryDisplayOrder.ToString())
        {
            ModelState.AddModelError(nameof(category.CategoryName), "Display Order cannot exactly match the Category Name.");
            return View();
        }

        if (!ModelState.IsValid)
        {
            return View(category);
        }

        _dbContext.Categories.Add(category);
        _dbContext.SaveChanges();
        return RedirectToAction("Index");
    }
}
