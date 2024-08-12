using BulkyBook.DataAccess.Abstracts;
using BulkyBook.Models.Masters;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.WebUI.Areas.Admin.Controllers.Masters;

[Area("Admin")]
[Authorize(Roles = SD.Role.Admin)]
public class CompanyController : Controller
{
    private const string Company_Image_Path = @"images\companys";
    private readonly IUnitOfWork _unitOfWork;

    public CompanyController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<Company> companies = _unitOfWork.Company.GetAll();

        return View(companies);
    }

    public IActionResult Upsert(int? id)
    {
        if (id is null or 0)
        {
            //create
            return View(new Company());
        }

        //update
        Company company = _unitOfWork.Company.Get(x => x.Id == id);
        return View(company);
    }

    [HttpPost]
    public IActionResult Upsert(Company company)
    {
        //Check for Model Validations.
        if (!ModelState.IsValid)
        {
            return View(company);
        }

        //Check if ISBN Already Exists.
        var existingCompany = _unitOfWork.Company
            .Get(x => x.Id != company.Id && x.Name == company.Name);
        if (existingCompany != null)
        {
            ModelState.AddModelError(nameof(company.Name), "Company name already exists.");
            return View(company);
        }

        if (company.Id == 0)
            _unitOfWork.Company.Add(company);
        else
            _unitOfWork.Company.Update(company);

        _unitOfWork.SaveChanges();

        TempData["Success"] = "Company saved successfully.";

        return RedirectToAction("Index");
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        IEnumerable<Company> companies = _unitOfWork.Company.GetAll();
        return Json(new { data = companies });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        Company? company = _unitOfWork.Company.Get(x => x.Id == id);
        if (company == null)
            return Json(new { success = false, message = "Error while deleting, company id not found." });

        _unitOfWork.Company.Remove(company);
        _unitOfWork.SaveChanges();

        return Json(new { success = true, message = "Delete successful." });
    }

    #endregion
}
