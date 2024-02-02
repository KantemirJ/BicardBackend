namespace BicardBackend.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ServiceType { get; set; }
        public string DoctorName { get; set; }
        public string Age { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public string? TimeAtSchedule { get; set; }
        public int? ConfirmedByUserId { get; set; }
    }
}
