using System.Security.Claims;
using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hachiko.Controllers;

[Authorize]
public class ShoppingCartController : Controller
{
    private IUnitOfWork _unitOfWork;
    private ShoppingCartViewModel ShoppingCartViewModel { get; set; }        

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
            OrderTotal = 0
        };
        
        //Calculate Order Total
        foreach (var item in ShoppingCartViewModel.ShoppingCarts)
        {
            ShoppingCartViewModel.OrderTotal += item.Product.Price * item.Count;
        }
        
        return View(ShoppingCartViewModel);
    }

    public IActionResult Summary()
    {
        return View();
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