﻿using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Models.Masters;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.WebUI.Areas.Admin.Controllers.Masters;

[Area("Admin")]
[Authorize(Roles = SD.Role.Admin)]
public class ProductController : Controller
{
    private const string Product_Image_Path = @"images\products\product-{0}";
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> products = _unitOfWork.Product
            .GetAll(includeProperties: nameof(_unitOfWork.Category));

        return View(products);
    }

    public IActionResult Upsert(int? id)
    {
        //ViewBag.CategoryList = CategoryList;
        //ViewData["CategoryList"] = CategoryList;
        ProductVM productVM = new()
        {
            Product = new Product(),
            CategoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.CategoryName,
                    Value = u.CategoryId.ToString()
                })
        };

        if (id is null or 0)
        {
            //create
            return View(productVM);
        }

        //update
        productVM.Product = _unitOfWork.Product
            .Get(x => x.Id == id, includeProperties: "ProductImages");
        return View(productVM);
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files)
    {
        //Check for Model Validations.
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

        //Check if ISBN Already Exists.
        var existingProduct = _unitOfWork.Product
            .Get(x => x.Id != productVM.Product.Id && x.ISBN == productVM.Product.ISBN);
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


        if (productVM.Product.Id == 0)
            _unitOfWork.Product.Add(productVM.Product);
        else
            _unitOfWork.Product.Update(productVM.Product);

        _unitOfWork.SaveChanges();

        if (files != null)
        {
            List<ProductImage> productImages = new List<ProductImage>();
            foreach (var file in files)
            {
                var imagePath = string.Format(Product_Image_Path, productVM.Product.Id);

                ProductImage productImage = new ProductImage()
                {
                    ImageUrl = UploadImages(file, imagePath),
                    ProductId = productVM.Product.Id,
                };
                productImages.Add(productImage);

                //_unitOfWork.ProductImage.Update(productImage);
            }

            productVM.Product.ProductImages = productImages;

            _unitOfWork.Product.Update(productVM.Product);
            _unitOfWork.SaveChanges();
        }

        //productVM.Product.ImageUrl = UploadImage(file, Product_Image_Path, productVM.Product.ImageUrl);


        TempData["Success"] = "Product saved successfully.";

        return RedirectToAction("Index");
    }

    private string UploadImages(IFormFile? file, string imagePath, string? existingImageUrl = null)
    {
        if (file == null)
            return string.Empty;

        string wwwRootPath = _webHostEnvironment.WebRootPath;
        if (!string.IsNullOrEmpty(existingImageUrl))
        {
            //delete the old image
            var oldImagePath = Path.Combine(wwwRootPath, existingImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }

        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var productPath = Path.Combine(wwwRootPath, imagePath);

        if (!Directory.Exists(productPath))
            Directory.CreateDirectory(productPath);

        using var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create);

        file.CopyTo(fileStream);

        return Path.Combine(@"\", imagePath, fileName);
    }

    private string UploadImage(IFormFile? file, string imagePath, string? existingImageUrl = null)
    {
        if (file == null)
            return string.Empty;

        string wwwRootPath = _webHostEnvironment.WebRootPath;
        if (!string.IsNullOrEmpty(existingImageUrl))
        {
            //delete the old image
            var oldImagePath = Path.Combine(wwwRootPath, existingImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }

        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var productPath = Path.Combine(wwwRootPath, imagePath);

        using var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create);

        file.CopyTo(fileStream);

        return Path.Combine(@"\", imagePath, fileName);
    }

    public IActionResult DeleteImage(int imageId)
    {
        var imageToBeDeleted = _unitOfWork.ProductImage.Get(x => x.Id == imageId);
        var productId = imageToBeDeleted.ProductId;

        if (imageToBeDeleted == null)
        {
            TempData["Error"] = "Unable to delete image. Image data not found.";

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }

        if (string.IsNullOrEmpty(imageToBeDeleted.ImageUrl) == false)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            //delete the old image
            var oldImagePath = Path.Combine(wwwRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }

        _unitOfWork.ProductImage.Remove(imageToBeDeleted);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Image deleted successfully.";

        return RedirectToAction(nameof(Upsert), new { id = productId });
    }

    //public IActionResult Create()
    //{
    //    //ViewBag.CategoryList = CategoryList;
    //    //ViewData["CategoryList"] = CategoryList;
    //    ProductVM productVM = new()
    //    {
    //        Product = new Product(),
    //        CategoryList = _unitOfWork.Category.GetAll().
    //            Select(u => new SelectListItem
    //            {
    //                Text = u.CategoryName,
    //                Value = u.CategoryId.ToString()
    //            })
    //    };

    //    return View(productVM);
    //}

    //[HttpPost]
    //public IActionResult Create(ProductVM productVM)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        productVM.CategoryList = _unitOfWork.Category.GetAll().
    //            Select(u => new SelectListItem
    //            {
    //                Text = u.CategoryName,
    //                Value = u.CategoryId.ToString()
    //            });

    //        return View(productVM);
    //    }

    //    var existingProduct = _unitOfWork.Product.GetFirstOrDefault(x => x.ISBN == productVM.Product.ISBN);
    //    if (existingProduct != null)
    //    {
    //        productVM.CategoryList = _unitOfWork.Category.GetAll().
    //            Select(u => new SelectListItem
    //            {
    //                Text = u.CategoryName,
    //                Value = u.CategoryId.ToString()
    //            });

    //        ModelState.AddModelError(nameof(productVM.Product.ISBN), "ISBN number already exists.");
    //        return View(productVM);
    //    }

    //    _unitOfWork.Product.Add(productVM.Product);
    //    _unitOfWork.SaveChanges();

    //    TempData["Success"] = "Product created successfully.";

    //    return RedirectToAction("Index");
    //}

    //public IActionResult Edit(int? id)
    //{
    //    if (id is null or 0)
    //        return NotFound();

    //    Product? product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
    //    if (product == null)
    //        return NotFound();

    //    return View(product);
    //}

    //[HttpPost]
    //public IActionResult Edit(Product product)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(product);
    //    }

    //    var existingProduct = _unitOfWork.Product.GetFirstOrDefault(x => x.Id != product.Id && x.ISBN == product.ISBN);
    //    if (existingProduct != null)
    //    {
    //        ModelState.AddModelError(nameof(product.ISBN), "ISBN number already exists for another Book.");
    //        return View();
    //    }

    //    _unitOfWork.Product.Update(product);
    //    _unitOfWork.SaveChanges();

    //    TempData["Success"] = "Product updated successfully.";

    //    return RedirectToAction("Index");
    //}

    //public IActionResult Delete(int? id)
    //{
    //    if (id is null or 0)
    //        return NotFound();

    //    Product? product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
    //    if (product == null)
    //        return NotFound();

    //    return View(product);
    //}

    //[HttpPost, ActionName("Delete")]
    //public IActionResult DeletePOST(int? id)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View();
    //    }
    //    Product? product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
    //    if (product == null)
    //        return NotFound();

    //    _unitOfWork.Product.Remove(product);
    //    _unitOfWork.SaveChanges();

    //    TempData["Success"] = "Product deleted successfully.";

    //    return RedirectToAction("Index");
    //}

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: nameof(_unitOfWork.Category));
        return Json(new { data = products });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        Product? product = _unitOfWork.Product.Get(x => x.Id == id);
        if (product == null)
            return Json(new { success = false, message = "Error while deleting, product id not found." });


        string wwwRootPath = _webHostEnvironment.WebRootPath;

        var imagePath = string.Format(Product_Image_Path, id);
        var productPath = Path.Combine(wwwRootPath, imagePath);

        if (Directory.Exists(productPath))
            Directory.Delete(productPath, true);

        _unitOfWork.Product.Remove(product);
        _unitOfWork.SaveChanges();

        return Json(new { success = true, message = "Delete successful." });
    }

    #endregion
}
