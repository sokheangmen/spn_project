namespace MyAPI.Models
{
    public class UsersProfile
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public string? Full_Name { get; set; }
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public int Phone { get; set; }
        public string? Preferences { get; set; }
        public Users User { get; set; }
    }
}