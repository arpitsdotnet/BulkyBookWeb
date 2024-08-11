using System.Diagnostics;
using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Models.Masters;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public OrderController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }


    public IActionResult Index()
    {
        return View();
    }


    #region API CALLS

    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeader
            .GetAll(includeProperties: nameof(_unitOfWork.ApplicationUser));

        switch (status)
        {
            case "pending":
                orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatus.DelayedPayment);
                break;
            case "inprocess":
                orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.OrderStatus.Pending);
                break;
            case "completed":
                orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.OrderStatus.Shipped);
                break;
            case "approved":
                orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.OrderStatus.Approved);
                break;
            default:
                break;
        }

        return Json(new { data = orderHeaders });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        OrderHeader? orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id);
        if (orderHeader == null)
            return Json(new { success = false, message = "Error while deleting, Order id not found." });

        //var oldImageUrl = orderHeader.ImageUrl;
        //if (!string.IsNullOrEmpty(oldImageUrl))
        //{
        //    string wwwRootPath = _webHostEnvironment.WebRootPath;
        //    //delete the old image
        //    var oldImagePath = Path.Combine(wwwRootPath, oldImageUrl.TrimStart('\\'));
        //    if (System.IO.File.Exists(oldImagePath))
        //    {
        //        System.IO.File.Delete(oldImagePath);
        //    }
        //}

        _unitOfWork.OrderHeader.Remove(orderHeader);
        _unitOfWork.SaveChanges();

        return Json(new { success = true, message = "Delete successful." });
    }

    #endregion
}
