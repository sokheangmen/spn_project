using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.Dtos;
using MyAPI.Services;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "Admin")]
public class APIUsersController : ControllerBase

{
    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        return Ok("You are authenticated.");
    }

    private readonly ApplicationDbContext _db;
    public APIUsersController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _db.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync();

        var result = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.user_name,
            Email = u.email,
            IsActive = u.is_active,
            CreatedAt = u.created_at,
            Roles = u.UserRoles.Select(ur => ur.Role.role_name).ToList()
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var u = await _db.Users
            .Include(x => x.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (u == null) return NotFound();

        var dto = new UserDto
        {
            Id = u.Id,
            Username = u.user_name,
            Email = u.email,
            IsActive = u.is_active,
            CreatedAt = u.created_at,
            Roles = u.UserRoles.Select(ur => ur.Role.role_name).ToList()
        };
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserDto model)
    {
        if (await _db.Users.AnyAsync(x => x.user_name == model.Username))
            return BadRequest("Username already exists.");

        var user = new Users
        {
            user_name = model.Username,
            email = model.Email,
            password = SecurityService.HashPassword(model.Password),
            created_at = DateTime.UtcNow,
            is_active = true
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new { user.Id, user.user_name });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateUser(int id, UpdateUserDto model)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.email = model.Email;
        user.is_active = model.IsActive;

        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        // remove userroles first (cascade may handle, but explicit is safe)
        var urs = _db.UserRoles.Where(ur => ur.user_Id == id);
        _db.UserRoles.RemoveRange(urs);

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Optional: change password
    [HttpPost("{id:int}/changepassword")]
    public async Task<ActionResult> ChangePassword(int id, [FromBody] string newPassword)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.password = SecurityService.HashPassword(newPassword);
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
