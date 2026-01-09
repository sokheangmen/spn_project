using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "Web")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Redirect /Admin → /Admin/Dashboard
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
