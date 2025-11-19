using System.Security.Claims;
using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Hachiko.ViewComponents;

public class ShoppingCartViewComponent : ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    private void UpdateCartSession(string userId)
    {
        HttpContext.Session.SetInt32(SD.SessionCart,
            _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
    }
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var claimsIdentity = (ClaimsIdentity) User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is not null)
        {
            if (HttpContext.Session.GetInt32(SD.SessionCart) is null)
            {
                UpdateCartSession(claim.Value);
            }
            return View(HttpContext.Session.GetInt32(SD.SessionCart));
        }
        else
        {
            HttpContext.Session.Clear();
            return View(0);
        }
    }
}