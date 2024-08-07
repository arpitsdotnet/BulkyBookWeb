using System.Security.Claims;
using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Models.Identity;
using BulkyBook.Models.Masters;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utilities;
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

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u =>
                                    u.ApplicationUserId == userId,
                                    includeProperties: nameof(_unitOfWork.Product))
        };

        ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.ShoppingCartList.Sum(x => x.GetTotalPriceBasedOnQuantity());

        return View(ShoppingCartVM);
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u =>
                                    u.ApplicationUserId == userId,
                                    includeProperties: nameof(_unitOfWork.Product))
        };

        double orderTotal = ShoppingCartVM.ShoppingCartList.Sum(x => x.GetTotalPriceBasedOnQuantity());

        var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

        ShoppingCartVM.OrderHeader = new OrderHeader
        {
            ApplicationUser = applicationUser,
            Name = applicationUser.Name,
            PhoneNumber = applicationUser.PhoneNumber,
            StreetAddress = applicationUser.StreetAddress,
            City = applicationUser.City,
            State = applicationUser.State,
            PostalCode = applicationUser.PostalCode,
            OrderTotal = orderTotal,
            EstimatedArrivalDate = $"{DateTime.Now.AddDays(7).ToString(SD.DateFormat.Date)} - {DateTime.Now.AddDays(14).ToString(SD.DateFormat.Date)}"
        };

        return View(ShoppingCartVM);
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
