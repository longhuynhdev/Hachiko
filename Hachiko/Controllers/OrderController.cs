using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.Models.ViewModels;
using Hachiko.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Hachiko.Controllers;

public class OrderController : Controller
{
    private IUnitOfWork _unitOfWork;

    public OrderController(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Details(int orderId)
    {
        OrderViewModel orderViewModel = new()
        {
            OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties:"ApplicationUser"),
            OrderDetails = _unitOfWork.OrderDetail.GetAll(u=>u.OrderHeaderId == orderId, includeProperties:"Product")
        };
        return View(orderViewModel);
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll(string status)
    {
        var orderList = _unitOfWork.OrderHeader.GetAll(includeProperties:"ApplicationUser");
        
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