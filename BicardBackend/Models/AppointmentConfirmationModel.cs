namespace BicardBackend.Models
{
    public class AppointmentConfirmationModel
    {
        public int Id { get; set; }
        public string TimeAtSchedule { get; set; }
        public int ConfirmedByUserId { get; set; }
    }
}
