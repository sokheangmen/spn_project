using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "Web")]
    public class PostsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
