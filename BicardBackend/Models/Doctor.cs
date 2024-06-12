namespace BicardBackend.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Speciality { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public string? PathToPhoto { get; set; }
        public int UserId { get; set; }
        public ICollection<SubMedServiceDoctor>? SubMedServiceDoctors { get; set; }
        public ICollection<Schedule>? Schedules { get; set; }
    }
}
