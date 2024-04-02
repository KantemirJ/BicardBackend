namespace BicardBackend.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string AuthorId { get; set; }
        public Doctor Author { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string PhotoPath { get; set; }

    }
}
