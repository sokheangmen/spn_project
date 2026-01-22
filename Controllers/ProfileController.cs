using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Data;
using System.Security.Claims;

[Authorize] // Ensure user is logged in
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _db;

    public ProfileController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        // 1️⃣ Get the user id from claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("Unauthorized"); 

        int userId = int.Parse(userIdClaim.Value);

        // 2️⃣ Find the profile by userId
        var profile = _db.UserProfile.FirstOrDefault(p => p.User_Id == userId);
        if (profile == null)
            return NotFound("Profile not found");

        // 3️⃣ Return view
        return View(profile);
    }
}
