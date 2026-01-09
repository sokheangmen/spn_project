using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult HomeIndex()
        {
            return View();
        }
    }
}
