namespace MyAPI.Models.Dtos
{
    public class UpdateProfileDto
    {
        public string full_Name { get; set; }
        public string user_name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string gender { get; set; }

        public IFormFile Avatar { get; set; } // image
    }

}
