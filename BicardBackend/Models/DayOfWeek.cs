namespace BicardBackend.Models
{
    public class DayOfWeek
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int EmployeeId { get; set; }
    }
}
