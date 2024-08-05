using System.Diagnostics;
using BulkyBook.DataAccess.Abstracts;
using BulkyBook.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.WebUI.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
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

        return View(product);
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
