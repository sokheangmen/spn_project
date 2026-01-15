namespace MyAPI.Models.Dtos
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsWeb { get; set; } = false;
    }
}
