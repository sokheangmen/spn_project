namespace MyAPI.Models.Dtos
{
    public class CreateUserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }    // raw password -> will be hashed
        public string Email { get; set; }
    }
}
