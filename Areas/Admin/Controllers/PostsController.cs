using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.Dtos;

[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PostController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ================= CREATE POST =================
    [HttpPost]
    [Authorize] // MUST be logged in
    public async Task<IActionResult> CreatePost([FromForm] PostsDto model)
    {
        // 1️⃣ Get logged-in userId safely
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim == null)
            return Unauthorized(new { message = "User not logged in" });

        if (!int.TryParse(claim.Value, out int userId))
            return BadRequest(new { message = "Invalid userId" });

        // 2️⃣ Create post safely
        var post = new Posts
        {
            user_id = userId,
            content = model.content ?? "",
            status = true,
            created_at = DateTime.Now,
            updated_at = DateTime.Now // <-- FIX
        };

        // 3️⃣ Handle image upload safely
        if (model.image != null)
        {
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/posts"
            );

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(model.image.FileName);

            // ✅ unique & correct name
            var fileName = $"Post_{post.Id}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.image.CopyToAsync(stream);
            }

            // ✅ correct public path
            post.image = "/uploads/posts/" + fileName;

            await _db.SaveChangesAsync();
        }


        // 4️⃣ Save to database
        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        // 5️⃣ Return response
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

    // ================= GET ALL POSTS =================
    [HttpGet]
    public IActionResult GetPosts()
    {
        var posts = _db.Posts
            .Where(p => p.status)
            .OrderByDescending(p => p.created_at)
            .Select(p => new
            {
                p.Id,
                p.content,
                p.image,
                p.status,
                p.created_at,
                user = p.User != null ? p.User.user_name : "Unknown"
            })
            .ToList();

        return Ok(posts);
    }

    //-------------------------------EDIT Post ----------------------------
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePost(int id, [FromForm] PostsDto model)
    {
        // 1️⃣ Get logged-in user
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim == null)
            return Unauthorized(new { message = "User not logged in" });

        int userId = int.Parse(claim.Value);

        // 2️⃣ Find post
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == id && p.status);
        if (post == null)
            return NotFound(new { message = "Post not found" });

        // 3️⃣ Owner check
        if (post.user_id != userId)
            return Forbid();

        // 4️⃣ Update content
        post.content = model.content ?? post.content;
        post.updated_at = DateTime.Now;

        // 5️⃣ Update image (optional)
        if (model.image != null)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var fileName = Guid.NewGuid() + Path.GetExtension(model.image.FileName);
            var path = Path.Combine(uploads, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await model.image.CopyToAsync(stream);

            post.image = "/uploads/" + fileName;
        }

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Post updated successfully",
            post
        });
    }

    //---------------------Deleted post---------------------------

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(int id)
    {
        // 1️⃣ Logged-in user
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim == null)
            return Unauthorized(new { message = "User not logged in" });

        int userId = int.Parse(claim.Value);

        // 2️⃣ Find post
        var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == id && p.status);
        if (post == null)
            return NotFound(new { message = "Post not found" });

        // 3️⃣ Owner check
        if (post.user_id != userId)
            return Forbid();

        // 4️⃣ Soft delete
        post.status = false;
        post.updated_at = DateTime.Now;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Post deleted successfully" });
    }


}
