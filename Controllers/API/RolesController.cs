using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Models;
using MyAPI.Models.Dtos;

[Route("api/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin")] // enable later
public class RolesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public RolesController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
    {
        var roles = await _db.Roles.ToListAsync();
        return Ok(roles.Select(r => new RoleDto { Id = r.Id, RoleName = r.role_name }));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoleDto>> GetRole(int id)
    {
        var r = await _db.Roles.FindAsync(id);
        if (r == null) return NotFound();
        return Ok(new RoleDto { Id = r.Id, RoleName = r.role_name });
    }

    [HttpPost]
    public async Task<ActionResult> CreateRole(CreateRoleDto model)
    {
        if (await _db.Roles.AnyAsync(x => x.role_name == model.RoleName))
            return BadRequest("Role already exists.");

        var role = new Roles { role_name = model.RoleName };
        _db.Roles.Add(role);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRole), new { id = role.Id }, new RoleDto { Id = role.Id, RoleName = role.role_name });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateRole(int id, CreateRoleDto model)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role == null) return NotFound();
        role.role_name = model.RoleName;
        _db.Roles.Update(role);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteRole(int id)
    {
        var role = await _db.Roles.FindAsync(id);
        if (role == null) return NotFound();

        var ur = _db.UserRoles.Where(x => x.role_Id == id);
        _db.UserRoles.RemoveRange(ur);

        var rp = _db.RolePermissions.Where(x => x.role_Id == id);
        _db.RolePermissions.RemoveRange(rp);

        _db.Roles.Remove(role);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
