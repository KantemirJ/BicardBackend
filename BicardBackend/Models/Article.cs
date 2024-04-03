namespace BicardBackend.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string AuthorName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
