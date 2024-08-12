using System.Security.Claims;
using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Models.Masters;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace BulkyBook.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
    [BindProperty]
    public OrderVM OrderVM { get; set; }

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

    public IActionResult Details(int orderId)
    {
        OrderVM = new OrderVM
        {
            OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: nameof(_unitOfWork.ApplicationUser)),
            OrderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderId, includeProperties: nameof(_unitOfWork.Product))
        };

        return View(OrderVM);
    }

    [HttpPost]
    [Authorize(Roles = SD.Role.Admin + "," + SD.Role.Employee)]
    public IActionResult UpdateOrderHeader()
    {
        var existingOrder = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id) ?? throw new Exception("Order now found.");

        existingOrder.Name = OrderVM.OrderHeader.Name;
        existingOrder.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
        existingOrder.StreetAddress = OrderVM.OrderHeader.StreetAddress;
        existingOrder.City = OrderVM.OrderHeader.City;
        existingOrder.State = OrderVM.OrderHeader.State;
        existingOrder.PostalCode = OrderVM.OrderHeader.PostalCode;

        if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            existingOrder.Carrier = OrderVM.OrderHeader.Carrier;
        if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            existingOrder.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;


        _unitOfWork.OrderHeader.Update(existingOrder);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Order has been updated successfully.";

        return RedirectToAction(nameof(Details), new { orderId = existingOrder.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role.Admin + "," + SD.Role.Employee)]
    public IActionResult StartProcessing()
    {
        _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.OrderStatus.InProcess);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Order has been updated successfully.";

        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role.Admin + "," + SD.Role.Employee)]
    public IActionResult ShipOrder()
    {
        var existingOrder = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);
        existingOrder.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        existingOrder.Carrier = OrderVM.OrderHeader.Carrier;
        existingOrder.OrderStatus = SD.OrderStatus.Shipped;
        existingOrder.ShippingDate = DateTime.Now;

        if (existingOrder.PaymentStatus == SD.PaymentStatus.DelayedPayment)
            existingOrder.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));

        _unitOfWork.OrderHeader.Update(existingOrder);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Order has been shipped successfully.";

        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role.Admin + "," + SD.Role.Employee)]
    public IActionResult CancelOrder()
    {
        var existingOrder = _unitOfWork.OrderHeader.Get(x=>x.Id == OrderVM.OrderHeader.Id);

        if (existingOrder.PaymentStatus == SD.PaymentStatus.Approved)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = existingOrder.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStatus(existingOrder.Id, SD.OrderStatus.Cancelled, SD.PaymentStatus.Refunded);
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateStatus(existingOrder.Id, SD.OrderStatus.Cancelled, SD.OrderStatus.Cancelled);
        }
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Order has been cancelled successfully.";

        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> orderHeaders = new List<OrderHeader>();

        if (User.IsInRole(SD.Role.Admin) || User.IsInRole(SD.Role.Employee))
        {
            orderHeaders = _unitOfWork.OrderHeader
            .GetAll(includeProperties: nameof(_unitOfWork.ApplicationUser));
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity.IsAuthenticated == false)
            {
                return Json(new { data = orderHeaders });
            }
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            orderHeaders = _unitOfWork.OrderHeader
                .GetAll(x => x.ApplicationUserId == userId, includeProperties: nameof(_unitOfWork.ApplicationUser));
        }

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
