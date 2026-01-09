using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
