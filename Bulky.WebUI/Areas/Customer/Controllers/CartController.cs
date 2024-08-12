using System.Security.Claims;
using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Models.Identity;
using BulkyBook.Models.Masters;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Tax;

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

    [BindProperty]
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

    [HttpPost]
    [ActionName("Summary")]
    public IActionResult SummaryPost()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u =>
                                    u.ApplicationUserId == userId,
                                    includeProperties: nameof(_unitOfWork.Product));

        double orderTotal = ShoppingCartVM.ShoppingCartList.Sum(x => x.GetTotalPriceBasedOnQuantity());


        ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
        ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
        ShoppingCartVM.OrderHeader.OrderTotal = orderTotal;

        var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            //Regular customer account and capture payment
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatus.Pending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatus.Pending;
        }
        else
        {
            //Company user account and payment can be delayed.
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatus.DelayedPayment;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatus.Approved;
        }

        _unitOfWork.OrderHeader.Update(ShoppingCartVM.OrderHeader);
        _unitOfWork.SaveChanges();

        var orderHeaderId = ShoppingCartVM.OrderHeader.Id;

        foreach (var cartItem in ShoppingCartVM.ShoppingCartList)
        {
            OrderDetail orderDetail = new()
            {
                ProductId = cartItem.ProductId,
                OrderHeaderId = orderHeaderId,
                Price = cartItem.Price,
                Count = cartItem.Count
            };
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.SaveChanges();
        }

        if (applicationUser.CompanyId.GetValueOrDefault() != 0)
        {
            //Company account does not required payment for 30 days.
            return RedirectToAction(nameof(OrderConfirmation), new { id = orderHeaderId });
        }

        //Regular customer account and capture payment via stripe pg
        //Work To Do

        var domain = "https://localhost:44331/";


        var customerOptions = new CustomerCreateOptions
        {
            Name = "Jenny Rosen",
            Address = new AddressOptions
            {
                Line1 = "510 Townsend St",
                PostalCode = "98140",
                City = "San Francisco",
                State = "CA",
                Country = "US",
            },
        };

        //Code copied from: https://docs.stripe.com/tax/checkout?lang=dotnet
        var options = new Stripe.Checkout.SessionCreateOptions
        {
            //LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
            //{
            //    new Stripe.Checkout.SessionLineItemOptions
            //    {
            //        Price = "{{PRICE_ID}}",
            //        Quantity = 2,
            //    },
            //},
            LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = $"{domain}Customer/Cart/OrderConfirmation?id={orderHeaderId}",
            CancelUrl = $"{domain}Customer/Cart/OrderCancelled?id={orderHeaderId}",
            AutomaticTax = new Stripe.Checkout.SessionAutomaticTaxOptions { Enabled = true }
        };

        foreach (var item in ShoppingCartVM.ShoppingCartList)
        {
            var sessionLineItem = new Stripe.Checkout.SessionLineItemOptions
            {
                PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)item.Price * 100, //$20.50 => 2050
                    Currency = "inr",
                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Title
                    }
                },
                Quantity = item.Count
            };
            options.LineItems.Add(sessionLineItem);
        }

        var service = new Stripe.Checkout.SessionService();
        Stripe.Checkout.Session session = service.Create(options);

        _unitOfWork.OrderHeader.UpdateStipePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
        _unitOfWork.SaveChanges();

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(StatusCodes.Status303SeeOther);
    }


    static Calculation CalculateTax(long orderAmount, string currency)
    {
        var calculationCreateOptions = new CalculationCreateOptions
        {
            Currency = currency,
            CustomerDetails = new CalculationCustomerDetailsOptions
            {
                Address = new AddressOptions
                {
                    Line1 = "920 5th Ave",
                    City = "Seattle",
                    State = "WA",
                    PostalCode = "98104",
                    Country = "US",
                },
                AddressSource = "shipping",
            },
            LineItems = new List<CalculationLineItemOptions> {
                 new() {
                    Amount = orderAmount,
                    Reference = "ProductRef",
                    TaxBehavior ="exclusive",
                    TaxCode = "txcd_30011000"
                }
            },
            ShippingCost = new CalculationShippingCostOptions { Amount = 300, TaxBehavior = "exclusive" },
        };

        var calculationService = new CalculationService();
        var calculation = calculationService.Create(calculationCreateOptions);

        return calculation;
    }

    public IActionResult OrderConfirmation(int id)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, includeProperties: nameof(_unitOfWork.ApplicationUser));
        if (orderHeader.PaymentStatus != SD.PaymentStatus.DelayedPayment)
        {
            //Order by Customer
            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStipePaymentID(id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.OrderStatus.Approved, SD.PaymentStatus.Approved);
                _unitOfWork.SaveChanges();
            }
        }

        List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
            .GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

        _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
        _unitOfWork.SaveChanges();


        return View(id);
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
        var existingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);
        try
        {
            existingCart.DecreaseCount(1);

            _unitOfWork.ShoppingCart.Update(existingCart);
            _unitOfWork.SaveChanges();
        }
        catch (ArgumentOutOfRangeException ex)
        {
            HttpContext.Session.SetInt32(SD.Session.ShoppingCart, _unitOfWork.ShoppingCart.GetAll(u =>
                    u.ApplicationUserId == existingCart.ApplicationUserId).Count() - 1);

            _unitOfWork.ShoppingCart.Remove(existingCart);
            _unitOfWork.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        // tracked: true: Enabling Tracking for Remove method to work,
        // cause remove method force tracking to be lost.
        ShoppingCart existingCart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked: true);

        HttpContext.Session.SetInt32(SD.Session.ShoppingCart, _unitOfWork.ShoppingCart.GetAll(u =>
                u.ApplicationUserId == existingCart.ApplicationUserId).Count() - 1);

        _unitOfWork.ShoppingCart.Remove(existingCart);
        _unitOfWork.SaveChanges();

        TempData["Success"] = "Cart item has been removed successfully.";

        return RedirectToAction(nameof(Index));
    }

}
