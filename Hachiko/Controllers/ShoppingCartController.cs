using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.Models;
using Hachiko.Models.ViewModels;
using Hachiko.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace Hachiko.Controllers;

[Authorize]
public class ShoppingCartController : Controller
{
    private IUnitOfWork _unitOfWork;
    [BindProperty]
    public ShoppingCartViewModel ShoppingCartViewModel { get; set; }        

    public ShoppingCartController(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        ShoppingCartViewModel = new ()
        {
            ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties:"Product"),
            OrderHeader = new()
        };
        
        //Calculate Order Total
        foreach (var item in ShoppingCartViewModel.ShoppingCarts)
        {
            ShoppingCartViewModel.OrderHeader.OrderTotal += item.Product.Price * item.Count;
            item.Price = item.Product.Price;
        }
        
        return View(ShoppingCartViewModel);
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        
        ShoppingCartViewModel = new()
        {
            ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
            OrderHeader = new()
        };

        ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
        ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
        ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
        ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
        ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
        ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;

        //Calculate Order Total
        foreach (var item in ShoppingCartViewModel.ShoppingCarts)
        {
            ShoppingCartViewModel.OrderHeader.OrderTotal += item.Product.Price * item.Count;
            item.Price = item.Product.Price;
        }

        return View(ShoppingCartViewModel);
    }

    [HttpPost]
    [ActionName("Summary")]
    public IActionResult SummaryPost()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;
        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        ShoppingCartViewModel.ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
        ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;

        foreach (var item in ShoppingCartViewModel.ShoppingCarts)
        {
            ShoppingCartViewModel.OrderHeader.OrderTotal += item.Product.Price * item.Count;
            item.Price = item.Product.Price;
        }

        ShoppingCartViewModel.OrderHeader.OrderStatus = SD.StatusPending;
        ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusPending;

        _unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
        _unitOfWork.Save();

        foreach (var item in ShoppingCartViewModel.ShoppingCarts)
        {
            OrderDetail orderDetail = new()
            {
                OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
                ProductId = item.ProductId,
                Count = item.Count,
                Price = item.Price
            };
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Save();
        }

        // Stripe Logic
        var domain = "https://localhost:7209";
        var options = new Stripe.Checkout.SessionCreateOptions
        {
            SuccessUrl = $"{domain}/ShoppingCart/OrderConfirmation?orderId={ShoppingCartViewModel.OrderHeader.Id}",
            CancelUrl = $"{domain}/ShoppingCart/Index",
            LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
            Mode = "payment",
        };

        foreach(var item in ShoppingCartViewModel.ShoppingCarts)
        {
            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price) * 100, //25.50$ => 2500
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
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
        // Current: session.PaymentIntentId == null
        _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
        _unitOfWork.Save();

        //TODO: Find out what this means
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);

        //return RedirectToAction(nameof(OrderConfirmation), new {orderId = ShoppingCartViewModel.OrderHeader.Id});
    }

    public IActionResult OrderConfirmation(int orderId)
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties:"ApplicationUser");

        var service = new SessionService();
        var session = service.Get(orderHeader.SessionId);

        if (session.PaymentStatus.ToLower() == "paid")
        {
            _unitOfWork.OrderHeader.UpdateStripePaymentID(orderId, session.Id, session.PaymentIntentId);
            _unitOfWork.OrderHeader.UpdateStatus(orderId, SD.StatusApproved, SD.PaymentStatusApproved);
            _unitOfWork.Save();

        }

        var shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
        _unitOfWork.Save();

        return View(orderId);
    }

    public IActionResult Plus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        cartFromDb.Count += 1;
        
        _unitOfWork.ShoppingCart.Update(cartFromDb);
        _unitOfWork.Save();
        
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        
        if (cartFromDb.Count <= 1)
        {
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
        }
        else
        {
            cartFromDb.Count -= 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }
        _unitOfWork.Save();
        
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
        _unitOfWork.ShoppingCart.Remove(cartFromDb);
        _unitOfWork.Save();
        
        return RedirectToAction(nameof(Index));
    }
}