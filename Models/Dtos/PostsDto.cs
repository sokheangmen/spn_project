namespace MyAPI.Models.Dtos
{
    public class PostsDto
    {
        public string content { get; set; }

        public IFormFile? image { get; set; }
    }
}
