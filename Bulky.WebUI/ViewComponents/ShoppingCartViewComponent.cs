using System.Security.Claims;
using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.WebUI.ViewComponents;

public class ShoppingCartViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            HttpContext.Session.Clear();
            return View(0);
        }

        if(HttpContext.Session.GetInt32(SD.Session.ShoppingCart) != null)
        {
            return View(HttpContext.Session.GetInt32(SD.Session.ShoppingCart));
        }

        HttpContext.Session.SetInt32(SD.Session.ShoppingCart, _unitOfWork.ShoppingCart.GetAll(u =>
            u.ApplicationUserId == userIdClaim.Value).Count());

        return View(HttpContext.Session.GetInt32(SD.Session.ShoppingCart));
    }
}
