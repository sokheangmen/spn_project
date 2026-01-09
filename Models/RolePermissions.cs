using System.Data;

namespace MyAPI.Models
{
    public class RolePermissions
    {
        public int role_Id { get; set; }
        public Roles Role { get; set; }

        public int permission_Id { get; set; }
        public  Permissions Permission { get; set; }
    }
}
