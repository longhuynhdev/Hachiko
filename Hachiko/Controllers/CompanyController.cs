using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.Models;
using Hachiko.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hachiko.Controllers;

[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    private IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var companies = _unitOfWork.Company.GetAll().ToList();
        return View("Index", companies);
    }

    public IActionResult Create()
    {
        return View("Create", new Company());
    }

    [HttpPost]
    public IActionResult Create(Company company)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Company.Add(company);
            _unitOfWork.Save();
            TempData["success"] = "Company created successfully";
            return RedirectToAction("Index");
        }

        return View("Create", company);
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }

        Company? company = _unitOfWork.Company.Get(c => c.Id == id);
        
        if (company is null)
        {
            return NotFound();
        }

        return View("Edit", company);
    }

    [HttpPost]
    public IActionResult Edit(Company company)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Company.Update(company);
            _unitOfWork.Save();
            TempData["success"] = "Company updated successfully";
            return RedirectToAction("Index");
        }

        return View(company);
    }

    public IActionResult Delete(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }

        Company? company = _unitOfWork.Company.Get(c => c.Id == id);

        if (company is null)
        {
            return NotFound();
        }
        
        return View("Delete", company);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteCompany(int? id)
    {
        if (id is null or 0)
        {
            return NotFound();
        }
        Company? company = _unitOfWork.Company.Get(c => c.Id == id);
        
        if (company is null)
        {
            return NotFound();
        }
        _unitOfWork.Company.Remove(company);
        _unitOfWork.Save();
        TempData["success"] = "Company deleted successfully";
        return RedirectToAction("Index");
    }
}