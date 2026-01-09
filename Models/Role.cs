namespace MyAPI.Models
{
    public class Roles
    {
        public int Id { get; set; }
        public required string role_name { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
        public ICollection<RolePermissions> RolePermissions { get; set; }
    }
}
