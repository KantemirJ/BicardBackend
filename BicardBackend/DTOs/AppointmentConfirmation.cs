namespace BicardBackend.DTOs
{
    public class AppointmentConfirmation
    {
        public int Id { get; set; }
        public string TimeAtSchedule { get; set; }
        public int ConfirmedByUserId { get; set; }
    }
}
