namespace MyAPI.Models
{
    public class Likes
    {
        public int Id { get; set; }
        public int user_id { get; set; }
        public int post_id { get; set; }
        public int video_id { get; set; }
        public DateTime? created_at { get; set; }
    }
}
