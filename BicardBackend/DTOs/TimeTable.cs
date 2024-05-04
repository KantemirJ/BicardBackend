namespace BicardBackend.DTOs
{
    public class TimeTable
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialty { get; set; }
        public List<TimeTableDay> Days { get;} = new List<TimeTableDay>();

    }
    public class TimeTableDay 
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
