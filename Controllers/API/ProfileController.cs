using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.Dtos;
using System;
using System.Threading.Tasks;

namespace MyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProfileController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET: api/Profile
        [HttpGet]
        public async Task<IActionResult> GetAllProfiles()
        {
            var profiles = await _db.UserProfile.ToListAsync();
            return Ok(profiles);
        }

        // GET: api/Profile/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var profile = await _db.UserProfile.FindAsync(id);
            if (profile == null)
                return NotFound(new { message = "Profile not found" });

            return Ok(profile);
        }

        // POST: api/Profile
        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] UserProfile profile)
        {
            profile.created_at = DateTime.Now;
            profile.updated_at = DateTime.Now;

            _db.UserProfile.Add(profile);
            await _db.SaveChangesAsync();

            return Ok(profile);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromForm] UpdateProfileDto profile)
        {
            var userProfiles = await _db.UserProfile.FindAsync(id);
            if (userProfiles == null)
                return NotFound(new { message = "Profile not found" });

            userProfiles.full_Name = profile.full_Name;
            userProfiles.user_name = profile.user_name;
            userProfiles.email = profile.email;
            userProfiles.phone = profile.phone;
            userProfiles.gender = profile.gender;
            userProfiles.updated_at = DateTime.Now;

            if (profile.Avatar != null)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/avatars");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(profile.Avatar.FileName);
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await profile.Avatar.CopyToAsync(stream);

                userProfiles.avatar = "/avatars/" + fileName;
            }

            await _db.SaveChangesAsync();
            return Ok(userProfiles);
        }



        [HttpPut("upload-avatar/{id}")]
        public async Task<IActionResult> UploadAvatar(int id, IFormFile avatar)
        {
            if (avatar == null || avatar.Length == 0)
                return BadRequest("No file uploaded");

            var profile = await _db.UserProfile.FindAsync(id);
            if (profile == null)
                return NotFound("Profile not found");

            // File validation
            var allowedExt = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(avatar.FileName).ToLower();

            if (!allowedExt.Contains(ext))
                return BadRequest("Only jpg, jpeg, png allowed");

            // Create folder
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "avatars");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Unique filename
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            // Save path to DB
            profile.avatar = $"/uploads/avatars/{fileName}";
            profile.updated_at = DateTime.Now;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Avatar uploaded successfully",
                avatar = profile.avatar
            });
        }

        // DELETE: api/Profile/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var existing = await _db.UserProfile.FindAsync(id);
            if (existing == null)
                return NotFound(new { message = "Profile not found" });

            _db.UserProfile.Remove(existing);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
