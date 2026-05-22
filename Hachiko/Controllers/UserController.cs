using Hachiko.DataAccess.Repository.IRepository;
using Hachiko.DataAcess.Data;
using Hachiko.Models;
using Hachiko.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hachiko.Controllers;

[Authorize(Roles = SD.Role_Admin)]
public class UserController : Controller
{
    private ApplicationDbContext _db;

    public UserController(ApplicationDbContext db)
    {
        this._db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll() {
        List<ApplicationUser> users = _db.ApplicationUsers.ToList();
        return Json(new {data = users });
    }

    #endregion
}