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

        TempData["Success"] = "Category created successfully.";

        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? categoryId)
    {
        if (categoryId is null or 0)
            return NotFound();

        //Category? category = _dbContext.Categories.Find(categoryId);
        Category? category = _dbContext.Categories.FirstOrDefault(x => x.CategoryId == categoryId);
        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost]
    public IActionResult Edit(Category category)
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

        _dbContext.Categories.Update(category);
        _dbContext.SaveChanges();

        TempData["Success"] = "Category updated successfully.";

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int? categoryId)
    {
        if (categoryId is null or 0)
            return NotFound();

        //Category? category = _dbContext.Categories.Find(categoryId);
        Category? category = _dbContext.Categories.FirstOrDefault(x => x.CategoryId == categoryId);
        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? categoryId)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        Category? category = _dbContext.Categories.FirstOrDefault(x => x.CategoryId == categoryId);
        if (category == null)
            return NotFound();

        _dbContext.Categories.Remove(category);
        _dbContext.SaveChanges();

        TempData["Success"] = "Category deleted successfully.";

        return RedirectToAction("Index");
    }
}
