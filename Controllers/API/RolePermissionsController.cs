using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.Dtos;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin")] // enable later
public class RolePermissionsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public RolePermissionsController(ApplicationDbContext db) => _db = db;

    [HttpPost("assign")]
    public async Task<ActionResult> AssignPermission(AssignPermissionDto model)
    {
        var role = await _db.Roles.FindAsync(model.RoleId);
        if (role == null) return NotFound("Role not found");

        var perm = await _db.Permissions.FindAsync(model.PermissionId);
        if (perm == null) return NotFound("Permission not found");

        var exists = await _db.RolePermissions.AnyAsync(x => x.role_Id == model.RoleId && x.permission_Id == model.PermissionId);
        if (exists) return BadRequest("Permission already assigned.");

        _db.RolePermissions.Add(new RolePermissions { role_Id = model.RoleId, permission_Id = model.PermissionId });
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("remove")]
    public async Task<ActionResult> RemovePermission(AssignPermissionDto model)
    {
        var rp = await _db.RolePermissions.FirstOrDefaultAsync(x => x.role_Id == model.RoleId && x.permission_Id == model.PermissionId);
        if (rp == null) return NotFound("Assignment not found.");

        _db.RolePermissions.Remove(rp);
        await _db.SaveChangesAsync();
        return Ok();
    }
}
