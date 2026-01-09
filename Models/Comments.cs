namespace MyAPI.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public int user_id { get; set; }
        public int post_id { get; set; }
        public string? comment { get; set; }
        public DateTime? created_at { get; set; }
    }
}
