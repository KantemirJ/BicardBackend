using System.ComponentModel.DataAnnotations;

namespace BicardBackend.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public User User { get; set; }
        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
