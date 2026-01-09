using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.Dtos;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin")] // enable later
public class UserRolesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public UserRolesController(ApplicationDbContext db) => _db = db;

    [HttpPost("assign")]
    public async Task<ActionResult> AssignRole(AssignRoleDto model)
    {
        var user = await _db.Users.FindAsync(model.UserId);
        if (user == null) return NotFound("User not found");

        var role = await _db.Roles.FindAsync(model.RoleId);
        if (role == null) return NotFound("Role not found");

        var exists = await _db.UserRoles.AnyAsync(ur => ur.user_Id == model.UserId && ur.user_Id == model.RoleId);
        if (exists) return BadRequest("Role already assigned.");

        var ur = new UserRoles { user_Id = model.UserId, role_Id = model.RoleId };
        _db.UserRoles.Add(ur);
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("remove")]
    public async Task<ActionResult> RemoveRole(AssignRoleDto model)
    {
        var ur = await _db.UserRoles.FirstOrDefaultAsync(x => x.user_Id == model.UserId && x.role_Id == model.RoleId);
        if (ur == null) return NotFound("Assignment not found.");

        _db.UserRoles.Remove(ur);
        await _db.SaveChangesAsync();
        return Ok();
    }
}
