using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.DataAcess.Data;
using Hachiko.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hachiko.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //Constructor
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categoryList = _unitOfWork.Category.GetAll().ToList();
            return View("Index", categoryList);
        }

        public IActionResult Create()
        {
            return View("Create", new Category());
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {   //check Model State
            /*
            if (obj.Name.ToLower() == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name","Name và Order Number không được giống nhau");
            }
            */

            if (ModelState.IsValid)
            {
                //add new Category by EF 
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();

                //TempData to message to client
                TempData["success"] = "Category created successfully";

                return RedirectToAction("Index", "Category");

            }

            return View(obj);
            /*Sau khi them Category chuyen huong den Action Index de xem thay doi*/
        }

        public IActionResult Edit(int? id)
        {
            //Validation
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.Get(u => u.Id == id);
            /*
            Category? category1 = _dbContext.Categories.FirstOrDefault(item => item.Id == id);
            Category? category2 = _dbContext.Categories.Where(item => item.Id == id).FirstOrDefault();
            */


            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                //eidt Category with EF 
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();

                TempData["success"] = "Category edited successfully";

                /*Sau khi them Category chuyen huong den Action Index de xem thay doi*/
                return RedirectToAction("Index", "Category");
            }

            return View(obj);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            //Validation
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.Get(u => u.Id == id);


            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {

            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();

            TempData["success"] = "Category deleted successfully";

            /*Sau khi them Category chuyen huong den Action Index de xem thay doi*/
            return RedirectToAction("Index", "Category");

        }

    }
}
