using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Services;

namespace MyAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "Web")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ===================== LIST / Index =====================
        // GET: Admin/Users
        //[HttpGet("")]
        //public IActionResult Index()
        //{
        //    return View();
        //}
        public async Task<IActionResult> Index([FromQuery] bool asJson = false)
        {
            var users = await _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();

            if (asJson)
            {
                var data = users.Select(u => new
                {
                    u.Id,
                    u.user_name,
                    u.email,
                    u.is_active,
                    Roles = u.UserRoles != null
                        ? u.UserRoles.Select(ur => ur.Role.role_name).ToList()
                        : new List<string>()
                });

                return Ok(data);
            }

            return View(users); // Make sure Views/Admin/Users/Index.cshtml exists
        }

        // ===================== CREATE =====================
        // GET: Admin/Users/Create
        //[HttpGet("Create")]
        //public IActionResult Create()
        //{
        //    return View(); // Views/Admin/Users/Create.cshtml
        //}

        //// POST: Admin/Users/Create
        //[HttpPost("Create")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Users model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    if (await _db.Users.AnyAsync(u => u.user_name == model.user_name))
        //    {
        //        ModelState.AddModelError("", "Username already exists");
        //        return View(model);
        //    }

        //    model.password = SecurityService.HashPassword(model.password);
        //    model.created_at = DateTime.UtcNow;
        //    model.is_active = true;

        //    _db.Users.Add(model);
        //    await _db.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index)); // Correct redirect
        //}

        // ===================== EDIT =====================
        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            return PartialView("_Edit", user);
        }

        // POST: Admin/Users/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Users model)
        {
            if (id != model.Id)
                return BadRequest();

            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            // Update editable fields
            user.email = model.email;
            user.is_active = model.is_active;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===================== DELETE =====================
        // POST: Admin/Users/Delete/5
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
