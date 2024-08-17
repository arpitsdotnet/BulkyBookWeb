using BulkyBook.DataAccess.Abstracts;
using BulkyBook.DataAccess.Base;
using BulkyBook.Models.Identity;
using BulkyBook.Models.Masters;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.WebUI.Areas.Admin.Controllers.Masters;

[Area("Admin")]
[Authorize(Roles = SD.Role.Admin)]
public class UserController : Controller
{
    private const string Company_Image_Path = @"images\companys";
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(
        IUnitOfWork unitOfWork,
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult RoleManagement(string userId)
    {
        var roles = _context.Roles.ToList();

        RoleManagementVM roleVM = new RoleManagementVM
        {
            User = _context.ApplicationUsers.Include(x => x.Company).FirstOrDefault(x => x.Id == userId),
            RoleList = roles.Select(i => new SelectListItem(i.Name, i.Name)),
            CompanyList = _context.Companies.Select(i => new SelectListItem(i.Name, i.Id.ToString()))
        };

        string roleId = _context.UserRoles.FirstOrDefault(x => x.UserId == userId).RoleId;
        roleVM.User.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;

        return View(roleVM);
    }

    [HttpPost]
    public IActionResult RoleManagement(RoleManagementVM roleVM)
    {
        string roleId = _context.UserRoles.FirstOrDefault(x => x.UserId == roleVM.User.Id).RoleId;

        var roles = _context.Roles.ToList();
        var oldRole = roles.FirstOrDefault(x => x.Id == roleId).Name;

        if (roleVM.User.Role != oldRole)
        {
            //Role was updated
            ApplicationUser applicationUser = _context.ApplicationUsers.FirstOrDefault(x => x.Id == roleVM.User.Id);
            if (roleVM.User.Role == SD.Role.Company)
            {
                applicationUser.CompanyId = roleVM.User.CompanyId;
            }
            if (oldRole == SD.Role.Company)
            {
                applicationUser.CompanyId = null;
            }

            _context.SaveChanges();

            _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(applicationUser, roleVM.User.Role).GetAwaiter().GetResult();
        }

        return RedirectToAction(nameof(Index));
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
