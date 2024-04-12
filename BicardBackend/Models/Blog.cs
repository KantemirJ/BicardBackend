namespace BicardBackend.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string PhotoPath { get; set; }

    }
}
