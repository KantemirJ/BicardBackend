namespace BicardBackend.DTOs
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int DoctorId { get; set; }
    }
}
