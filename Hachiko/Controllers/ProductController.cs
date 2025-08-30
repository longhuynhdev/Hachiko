using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.Models;
using Hachiko.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hachiko.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //Inject IWebHostEnvironment using Dependency Injection
        private readonly IWebHostEnvironment _webHostEnvironment; //using to access wwwroot folder
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            
             
            return View("Index1",productList);
        }

        public IActionResult UpdateAndInsert(int? id)
        {
            //Get CategoryList
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.
                GetAll().Select(
              u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() }
                );
            //Pass CategoryList to View by ViewBag
            //ViewBag.CategoryList = CategoryList;
            ProductViewModel productVM = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = CategoryList
            };
            if (id == null || id == 0)
            {
                return View("UpdateAndInsert", productVM);
            } else
            {
                //Update 
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View("UpdateAndInsert", productVM);
            }
        }

        [HttpPost]
        public IActionResult UpdateAndInsert(ProductViewModel productVM, IFormFile? file)
        {  

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string productPath = Path.Combine(wwwRootPath + @"\images\product");
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    if(String.IsNullOrEmpty(productVM.Product.ImageUrl) == false)
                    {
                        //Remove old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileSteam = new FileStream(Path.Combine(productPath,fileName),FileMode.Create)) 
                    { 
                        file.CopyTo(fileSteam);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if(productVM.Product.Id == 0)
                {
                    //add new Product by EF 
                    //Insert
                    _unitOfWork.Product.Add(productVM.Product);
                } else
                {
                    //Update
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();

                //TempData to message to client
                TempData["success"] = "Category created successfully";

                return RedirectToAction("Index", "Product");

            } else
            {
                productVM.CategoryList = _unitOfWork.Category.
               GetAll().Select(
             u => new SelectListItem { 
                 Text = u.Name, 
                 Value = u.Id.ToString() 
                });
                
                return View(productVM);
            }

           
            /*Sau khi them Category chuyen huong den Action Index de xem thay doi*/
        }

        
        public IActionResult Delete(int? id)
        {
            //Validation
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? product = _unitOfWork.Product.Get(u => u.Id == id);


            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            //TODO: Handle delete image file
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();

            TempData["success"] = "Category deleted successfully";

            /*Sau khi them Category chuyen huong den Action Index de xem thay doi*/
            return RedirectToAction("Index", "Product");

        }

        #region API CALLS
        public IActionResult GetAll()
        {
            var objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult DeleteAPI(int? id)
        {
            var product = _unitOfWork.Product.Get(u => u.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var oldImagePath = 
                Path.Combine(_webHostEnvironment.WebRootPath, 
                product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();

            return Json(new {success= true, message = "Delete Successful"});
        }

        #endregion
    }
}
