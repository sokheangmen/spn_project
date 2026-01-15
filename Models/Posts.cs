using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Models
{
    public class Posts
    {

        public int Id { get; set; }
        public int user_id { get; set; }
        public string? content { get; set; }
        public string? image {  get; set; }
        public bool status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        [ForeignKey("user_id")]
        public Users? User { get; set; }
    }
}
