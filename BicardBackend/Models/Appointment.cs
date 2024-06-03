namespace BicardBackend.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Age { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public DateTime Date { get; set; }
        public int DoctorId { get; set; }
        public int? UserId { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
