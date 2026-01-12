using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


public class AuthController : Controller
{
    private readonly ApplicationDbContext _db;

    public AuthController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ================= LOGIN =================
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "Username and password are required";
            return View();
        }

        //  Get user by username only
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u =>
                u.user_name == username &&
                u.is_active);

        // Check user & password
        if (user == null )
        {
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        //  Create claims
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.user_name)
    };

        // Add ALL roles
        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.role_name));
        }

        // Create identity & sign in
        var identity = new ClaimsIdentity(claims, "Web");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("Web", principal);

        //  Save session (optional but OK)
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Username", user.user_name);

        //  Admin redirect
        bool isAdmin = user.UserRoles.Any(r => r.Role.role_name == "Admin");

        if (isAdmin)
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        //  Normal user redirect
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl != "/")
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("HomeIndex", "Home");
    }




    // ================= REGISTER =================
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        if (await _db.Users.AnyAsync(u => u.user_name == username))
        {
            ViewBag.Error = "Username already exists";
            return View();
        }

        var user = new Users
        {
            user_name = username,
            email = email,
            password = HashPassword(password),
            created_at = DateTime.Now,
            is_active = true,
             role = "User"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return RedirectToAction("Login");
    }

    // ================= LOGOUT =================
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    // ================= PASSWORD HASH =================
    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(sha.ComputeHash(bytes));
    }
}
