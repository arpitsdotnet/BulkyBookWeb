using BulkyBook.DataAccess.Abstracts;
using BulkyBook.DataAccess.Base;
using BulkyBook.Models.Identity;
using BulkyBook.Models.Masters;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.WebUI.Areas.Admin.Controllers.Masters;

[Area("Admin")]
[Authorize(Roles = SD.Role.Admin)]
public class UserController : Controller
{
    private const string Company_Image_Path = @"images\companys";
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public UserController(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }


    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        IEnumerable<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll(includeProperties: nameof(_unitOfWork.Company));

        var userRoles = _context.UserRoles.ToList();
        var roles = _context.Roles.ToList();

        foreach (var user in users)
        {
            var roleId = userRoles.FirstOrDefault(x => x.UserId == user.Id)?.RoleId;
            user.Role = roles.FirstOrDefault(x => x.Id == roleId)?.Name;

            if (user.Company == null)
            {
                user.Company = new Company { Name = "" };
            }
        }

        return Json(new { data = users });
    }

    [HttpPost]
    public IActionResult LockUnlock([FromBody] string id)
    {
        ApplicationUser? user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
        if (user == null)
            return Json(new { success = false, message = "Error while Locking/Unlocking." });

        string message = "";
        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
        {
            //user is currently locked and we need to unlock them
            user.LockoutEnd = DateTime.Now;
            message = "User is unlocked now.";
        }
        else
        {
            //lock user for 7 days
            user.LockoutEnd = DateTime.Now.AddDays(7);
            message = "User is locked for 7 days.";
        }
        
        _context.SaveChanges();

        return Json(new { success = true, message = message });
    }

    #endregion
}
