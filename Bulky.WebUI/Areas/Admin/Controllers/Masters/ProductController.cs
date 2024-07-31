using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Models.Masters;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.WebUI.Areas.Admin.Controllers.Masters;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> products = _unitOfWork.Product.GetAll();

        //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().
        //    Select(u => new SelectListItem
        //    {
        //        Text = u.CategoryName,
        //        Value = u.CategoryId.ToString()
        //    });

        //ViewBag.CategoryList = CategoryList;

        return View(products);
    }

    public IActionResult Create()
    {
        //ViewBag.CategoryList = CategoryList;
        //ViewData["CategoryList"] = CategoryList;
        ProductVM productVM = new()
        {
            Product = new Product(),
            CategoryList = _unitOfWork.Category.GetAll().
                Select(u => new SelectListItem
                {
                    Text = u.CategoryName,
                    Value = u.CategoryId.ToString()
                })
        };

        return View(productVM);
    }

    [HttpPost]
    public IActionResult Create(ProductVM productVM)
    {
        if (!ModelState.IsValid)
        {
            productVM.CategoryList = _unitOfWork.Category.GetAll().
                Select(u => new SelectListItem
                {
                    Text = u.CategoryName,
                    Value = u.CategoryId.ToString()
                });

            return View(productVM);
        }

        var existingProduct = _unitOfWork.Product.GetFirstOrDefault(x => x.ISBN == productVM.Product.ISBN);
        if (existingProduct != null)
        {
            productVM.CategoryList = _unitOfWork.Category.GetAll().
                Select(u => new SelectListItem
                {
                    Text = u.CategoryName,
                    Value = u.CategoryId.ToString()
                });

            ModelState.AddModelError(nameof(productVM.Product.ISBN), "ISBN number already exists.");
            return View(productVM);
        }

        _unitOfWork.Product.Add(productVM.Product);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Product created successfully.";

        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0)
            return NotFound();

        Product? product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var existingProduct = _unitOfWork.Product.GetFirstOrDefault(x => x.Id != product.Id && x.ISBN == product.ISBN);
        if (existingProduct != null)
        {
            ModelState.AddModelError(nameof(product.ISBN), "ISBN number already exists for another Book.");
            return View();
        }

        _unitOfWork.Product.Update(product);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Product updated successfully.";

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int? id)
    {
        if (id is null or 0)
            return NotFound();

        Product? product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        Product? product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
        if (product == null)
            return NotFound();

        _unitOfWork.Product.Remove(product);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Product deleted successfully.";

        return RedirectToAction("Index");
    }
}
