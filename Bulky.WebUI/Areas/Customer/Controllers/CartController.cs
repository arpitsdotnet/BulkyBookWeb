using System.Security.Claims;
using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Models.Masters;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.WebUI.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public ShoppingCartVM ShoppingCartVM { get; set; }

    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        IEnumerable<ShoppingCart> cartItems = _unitOfWork.ShoppingCart.GetAll(u =>
                        u.ApplicationUserId == userId,
                        includeProperties: nameof(_unitOfWork.Product));

        double orderTotal = 0;
        foreach (var item in cartItems)
        {
            orderTotal += item.GetTotalPriceBasedOnQuantity();
        }

        ShoppingCartVM = new()
        {
            ShoppingCartList = cartItems,
            OrderTotal = orderTotal
        };

        return View(ShoppingCartVM);
    }

    public IActionResult Summary()
    {
        return View();
    }

    public IActionResult Plus(int cartId)
    {
        var existingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        existingCart.IncreaseCount(1);

        _unitOfWork.ShoppingCart.Update(existingCart);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Item count has been increased successfully.";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        var existingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        try
        {
            existingCart.DecreaseCount(1);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _unitOfWork.ShoppingCart.Remove(existingCart);
            _unitOfWork.SaveChanges();

            TempData["Success"] = "Cart item has been removed successfully.";

            return RedirectToAction(nameof(Index));
        }

        _unitOfWork.ShoppingCart.Update(existingCart);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Item count has been decreased successfully.";

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var existingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        
        _unitOfWork.ShoppingCart.Remove(existingCart);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Cart item has been removed successfully.";

        return RedirectToAction(nameof(Index));
    }

}
