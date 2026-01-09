

namespace MyAPI.Models
{
    public class Permissions
    {
        public int Id { get; set; }
        public string permission_name { get; set; }

        public ICollection<RolePermissions> RolePermissions { get; set; }
    }
}
