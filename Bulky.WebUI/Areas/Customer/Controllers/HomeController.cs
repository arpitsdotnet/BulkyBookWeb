using System.Diagnostics;
using System.Security.Claims;
using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Models.Masters;
using BulkyBook.Utilities;
using BulkyBook.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.WebUI.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private const int ShoppingCart_Default_Count = 1;
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(
        ILogger<HomeController> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        if(userIdClaim != null)
        {
            HttpContext.Session.SetInt32(SD.Session.ShoppingCart, _unitOfWork.ShoppingCart.GetAll(u =>
                u.ApplicationUserId == userIdClaim.Value).Count());
        }

        var products = _unitOfWork.Product.GetAll(includeProperties: nameof(_unitOfWork.Category));

        return View(products);
    }

    public IActionResult Details(int productId)
    {
        var product = _unitOfWork.Product.Get(u => u.Id == productId, nameof(_unitOfWork.Category));

        if (product == null)
        {
            ModelState.AddModelError(nameof(product), "Unable to find the product");
            return View();
        }

        var shoppingCart = new ShoppingCart
        {
            ProductId = productId,
            Product = product,
            Count = ShoppingCart_Default_Count,
        };

        return View(shoppingCart);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        shoppingCart.ApplicationUserId = userId;

        ShoppingCart existingCart = _unitOfWork.ShoppingCart.Get(u => 
            u.ApplicationUserId == userId && 
            u.ProductId == shoppingCart.ProductId);

        if (existingCart != null)
        {
            
            existingCart.IncreaseCount(shoppingCart.Count);

            _unitOfWork.ShoppingCart.Update(existingCart);
            _unitOfWork.SaveChanges();
        }
        else
        {
            _unitOfWork.ShoppingCart.Add(shoppingCart);
            _unitOfWork.SaveChanges();

            HttpContext.Session.SetInt32(SD.Session.ShoppingCart, _unitOfWork.ShoppingCart.GetAll(u =>
                u.ApplicationUserId == userId).Count());
        }


        TempData["Success"] = "Cart saved successfully.";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
