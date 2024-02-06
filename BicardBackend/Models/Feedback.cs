namespace BicardBackend.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
