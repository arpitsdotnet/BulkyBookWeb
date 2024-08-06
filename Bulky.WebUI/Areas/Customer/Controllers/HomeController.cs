using System.Diagnostics;
using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Models.Masters;
using BulkyBook.WebUI.Models;
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
        var products = _unitOfWork.Product.GetAll(nameof(_unitOfWork.Category));

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
