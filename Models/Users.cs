using MyAPI.Data;

namespace MyAPI.Models
{
    public class Users
    {
        internal readonly string password_hash;

        public int Id { get; set; }
        public string user_name { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public bool is_active { get; set; }
        public DateTime created_at { get; set; }
        public bool is_deleted { get; set; }
        public int? deleted_by { get; set; }

        public string role { get; set; } = "User";
        public UsersProfile? UserProfile { get; set; }
        public  ICollection<UserRoles> UserRoles { get; set; }
    }


  
}
