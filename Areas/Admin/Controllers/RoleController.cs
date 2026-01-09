using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models.Dtos;

namespace MyAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RoleController(ApplicationDbContext db)
        {
            _db = db;
        }

        //// URL: /Admin/Role
        //[HttpGet("")]
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var roles = _db.Roles
                .AsNoTracking()
                .ToList();

            return View(roles);
        }
    }
}
