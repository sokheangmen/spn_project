using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.Dtos;
using System.Security.Claims;

namespace MyAPI.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public PostController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost("create")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create([FromForm] PostsDto model)
        {

            // 1️⃣ Get user id from token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "User not logged in" });

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return BadRequest(new { message = "Invalid userId" });

            // 2️⃣ Create post (NO image yet)
            var post = new Posts
            {
                user_id = userId,
                content = model.content ?? "",
                status = true,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            // 3️⃣ Handle image upload
            if (model.image != null)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/posts"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(model.image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.image.CopyToAsync(stream);
                }

                post.image = "/uploads/posts/" + fileName;
                await _db.SaveChangesAsync();
            }
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            // 4️⃣ Response
            return Ok(new
            {
                message = "Post created successfully",
                post = new
                {
                    post.user_id,
                    post.content,
                    post.image,
                    post.status,
                    post.created_at
                }
            });
        }
    }
}
