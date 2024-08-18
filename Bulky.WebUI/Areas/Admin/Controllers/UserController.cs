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
using static BulkyBook.Utilities.SD;

namespace BulkyBook.WebUI.Areas.Admin.Controllers.Masters;

[Area("Admin")]
[Authorize(Roles = SD.Role.Admin)]
public class UserController : Controller
{
    private const string Company_Image_Path = @"images\companys";
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(
        IUnitOfWork unitOfWork,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult RoleManagement(string userId)
    {

        ApplicationUser user = _unitOfWork.ApplicationUser.Get(x => x.Id == userId, includeProperties: "Company");
        user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

        List<IdentityRole> roles = _roleManager.Roles.ToList();
        IEnumerable<Company> companies = _unitOfWork.Company.GetAll();

        RoleManagementVM roleVM = new RoleManagementVM
        {
            User = user,
            RoleList = roles.Select(i => new SelectListItem(i.Name, i.Name)),
            CompanyList = companies.Select(i => new SelectListItem(i.Name, i.Id.ToString()))
        };

        return View(roleVM);
    }

    [HttpPost]
    public IActionResult RoleManagement(RoleManagementVM roleVM)
    {
        var oldRole = _userManager.GetRolesAsync(roleVM.User).GetAwaiter().GetResult().FirstOrDefault();

        ApplicationUser user = _unitOfWork.ApplicationUser.Get(x => x.Id == roleVM.User.Id);

        if (roleVM.User.Role != oldRole)
        {
            //Role was updated
            if (roleVM.User.Role == SD.Role.Company)
            {
                user.CompanyId = roleVM.User.CompanyId;
            }
            if (oldRole == SD.Role.Company)
            {
                user.CompanyId = null;
            }

            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.SaveChanges();

            _userManager.RemoveFromRoleAsync(user, oldRole).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, roleVM.User.Role).GetAwaiter().GetResult();
        }
        else if (oldRole == SD.Role.Company && user.CompanyId != roleVM.User.CompanyId)
        {
            user.CompanyId = roleVM.User.CompanyId;

            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }


    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        IEnumerable<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll(includeProperties: nameof(_unitOfWork.Company));

        foreach (var user in users)
        {
            user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

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
        ApplicationUser? user = _unitOfWork.ApplicationUser.Get(x => x.Id == id);
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

        _unitOfWork.ApplicationUser.Update(user);
        _unitOfWork.SaveChanges();

        return Json(new { success = true, message = message });
    }

    #endregion
}
