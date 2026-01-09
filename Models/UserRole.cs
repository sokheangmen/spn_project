using System.Data;

namespace MyAPI.Models
{
    public class UserRoles
    {
        internal object? User;

        public int user_Id { get; set; }
        public Users Users { get; set; }
        public int role_Id { get; set; }
        public Roles Role { get; set; }
    }
}
