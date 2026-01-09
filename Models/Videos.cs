namespace MyAPI.Models
{
    public class Videos
    {
        public int Id { get; set; }
        public int user_id { get; set; }
        public string? video_url { get; set; }
        public string? caption { get; set; }
        public int views { get; set; }
        public bool? status { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get;set; }
    }
}
