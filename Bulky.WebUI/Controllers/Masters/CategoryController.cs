﻿using Bulky.DataAccess.Abstracts.Masters;
using Bulky.DataAccess.Base;
using Bulky.Models.Masters;
using Microsoft.AspNetCore.Mvc;

namespace Bulky.WebUI.Controllers.Masters;
public class CategoryController : Controller
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoryController(
        ICategoryRepository categoryRepo
        )
    {
        _categoryRepo = categoryRepo;
    }
    public IActionResult Index()
    {
        IEnumerable<Category> categories = _categoryRepo.GetAll();

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

        var existingcategory = _categoryRepo.GetFirstOrDefault(x => x.CategoryName == category.CategoryName);
        if (existingcategory != null && existingcategory.CategoryName == category.CategoryName)
        {
            ModelState.AddModelError(nameof(category.CategoryName), "Category Name already exists.");
            return View();
        }


        _categoryRepo.Add(category);
        _categoryRepo.SaveChanges();

        TempData["Success"] = "Category created successfully.";

        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? categoryId)
    {
        if (categoryId is null or 0)
            return NotFound();

        //Category? category = _dbContext.Categories.Find(categoryId);
        Category? category = _categoryRepo.GetFirstOrDefault(x => x.CategoryId == categoryId);
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

        _categoryRepo.Update(category);
        _categoryRepo.SaveChanges();

        TempData["Success"] = "Category updated successfully.";

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int? categoryId)
    {
        if (categoryId is null or 0)
            return NotFound();

        //Category? category = _dbContext.Categories.Find(categoryId);
        Category? category = _categoryRepo.GetFirstOrDefault(x => x.CategoryId == categoryId);
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
        Category? category = _categoryRepo.GetFirstOrDefault(x => x.CategoryId == categoryId);
        if (category == null)
            return NotFound();

        _categoryRepo.Remove(category);
        _categoryRepo.SaveChanges();

        TempData["Success"] = "Category deleted successfully.";

        return RedirectToAction("Index");
    }
}
