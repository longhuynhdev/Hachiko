using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Hachiko.Utility;
using Microsoft.AspNetCore.Authorization;

namespace Hachiko.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }

        private void UpdateCartSession(string userId)
        {
            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is not null)
            {
                UpdateCartSession(claim.Value);
            }

            List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View("Index",productList);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart shoppingCart = new()
            {
                ProductId = id,
                Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category"),
                Count = 1
            };
            return View("Details",shoppingCart);
        }
        
        //TODO: Learn about ClaimsIdentity and how it works
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            // Get the user id of the logged-in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            shoppingCart.ApplicationUserId = userId;
            
            // Check if the product already exists in the shopping cart for this user
            ShoppingCart? cartFromDb = _unitOfWork.ShoppingCart.Get(
                u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
            
            if (cartFromDb is not null)
            {
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
            }

            UpdateCartSession(userId);
            
            TempData["success"] = "Product added to cart successfully!";
            
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View("Privacy");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}