using System.Security.Claims;
using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.Models;
using Hachiko.Models.ViewModels;
using Hachiko.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Hachiko.Controllers;

[Authorize]
public class OrderController : Controller
{
    private IUnitOfWork _unitOfWork;
    [BindProperty]
    public OrderViewModel orderViewModel { get; set; }

    public OrderController(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(int orderId)
    {
        //TODO: If the user is not admin, verify that the order belongs to the user
        
        orderViewModel = new() {
            OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties:"ApplicationUser"),
            OrderDetails = _unitOfWork.OrderDetail.GetAll(u=>u.OrderHeaderId == orderId, includeProperties:"Product")
        };
        return View(orderViewModel);
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult UpdateOrderDetails()
    {
        //TODO: Optimize update logic
        var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderViewModel.OrderHeader.Id);
        orderHeaderFromDb.Name = orderViewModel.OrderHeader.Name;
        orderHeaderFromDb.PhoneNumber = orderViewModel.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAddress = orderViewModel.OrderHeader.StreetAddress;
        orderHeaderFromDb.City = orderViewModel.OrderHeader.City;
        orderHeaderFromDb.State = orderViewModel.OrderHeader.State;
        orderHeaderFromDb.PostalCode = orderViewModel.OrderHeader.PostalCode;
        if (!string.IsNullOrWhiteSpace(orderHeaderFromDb.Carrier))
        {
            orderHeaderFromDb.Carrier = orderViewModel.OrderHeader.Carrier;
        }

        if (!string.IsNullOrWhiteSpace(orderHeaderFromDb.TrackingNumber))
        {
            orderHeaderFromDb.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
        }
        
        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();
        
        TempData["Success"] = "Order details updated successfully.";
        
        return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult ProcessingOrder()
    {
        _unitOfWork.OrderHeader.UpdateStatus(orderViewModel.OrderHeader.Id, SD.StatusInProcess);
        _unitOfWork.Save();
        
        TempData["Success"] = "Order processed successfully.";
        return RedirectToAction(nameof(Details), new { orderId = orderViewModel.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult ShipOrder()
    {
        //TODO: When shipping order, notify user via email with tracking number and carrier info, and update inventory accordingly.
        var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderViewModel.OrderHeader.Id);
        orderHeaderFromDb.Carrier = orderViewModel.OrderHeader.Carrier;
        orderHeaderFromDb.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
        orderHeaderFromDb.OrderStatus = SD.StatusShipped;
        orderHeaderFromDb.OrderShippingDate = DateTime.Now;
        
        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();

        TempData["Success"] = "Order shipped successfully.";
        return RedirectToAction(nameof(Details), new { orderId = orderViewModel.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult CancelOrder()
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderViewModel.OrderHeader.Id);
        if (orderHeaderFromDb.OrderStatus == SD.PaymentStatusApproved)
        {
            // Refund to customer via Stripe
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeaderFromDb.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);
            
            _unitOfWork.OrderHeader.UpdateStatus(orderHeaderFromDb.Id, SD.StatusRefunded);
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderHeaderFromDb.Id, SD.StatusCancelled);
        }
        _unitOfWork.Save();
        TempData["Success"] = "Order cancelled successfully.";
        return RedirectToAction(nameof(Details), new { orderId = orderViewModel.OrderHeader.Id });
    }
    

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> orderList;

        // Admin can see all orders, other users can see only their own orders
        if (User.IsInRole(SD.Role_Admin))
        {
            orderList = _unitOfWork.OrderHeader.GetAll(includeProperties:"ApplicationUser");
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            orderList = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUser.Id == userId, includeProperties:"ApplicationUser");
        }
        
        switch (status) {
            case "pending":
                orderList = orderList.Where(u => u.OrderStatus == SD.StatusPending);
                break;
            case "inprocess":
                orderList = orderList.Where(u => u.OrderStatus == SD.StatusInProcess);
                break;
            case "completed":
                orderList = orderList.Where(u => u.OrderStatus == SD.StatusShipped);
                break;
            case "approved":
                orderList = orderList.Where(u => u.OrderStatus == SD.StatusApproved);
                break;
        }
        
        return Json(new {data = orderList});
    }

    #endregion
}