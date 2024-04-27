namespace BicardBackend.DTOs
{
    public class ScheduleDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int DoctorId { get; set; }
    }
}
