using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.Dtos;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin")] // enable later
public class PermissionsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public PermissionsController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
    {
        var ps = await _db.Permissions.ToListAsync();
        return Ok(ps.Select(p => new PermissionDto { Id = p.Id, PermissionName = p.permission_name }));
    }

    [HttpPost]
    public async Task<ActionResult> CreatePermission(CreatePermissionDto model)
    {
        if (await _db.Permissions.AnyAsync(p => p.permission_name == model.PermissionName))
            return BadRequest("Permission already exists.");

        var p = new Permissions { permission_name = model.PermissionName };
        _db.Permissions.Add(p);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPermissions), new { id = p.Id }, new PermissionDto { Id = p.Id, PermissionName = p.permission_name });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePermission(int id)
    {
        var p = await _db.Permissions.FindAsync(id);
        if (p == null) return NotFound();

        var rp = _db.RolePermissions.Where(x => x.permission_Id == id);
        _db.RolePermissions.RemoveRange(rp);

        _db.Permissions.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
