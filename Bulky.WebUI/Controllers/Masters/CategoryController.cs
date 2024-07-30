using Microsoft.AspNetCore.Mvc;

namespace Bulky.WebUI.Controllers.Masters;
public class CategoryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
