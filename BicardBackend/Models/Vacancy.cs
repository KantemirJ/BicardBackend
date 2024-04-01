namespace BicardBackend.Models
{
    public class Vacancy
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public string Requirements { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
