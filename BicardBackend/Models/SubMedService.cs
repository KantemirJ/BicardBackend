namespace BicardBackend.Models
{
    public class SubMedService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public int MedServiceId { get; set; }
        public MedService MedService { get; set; }
        public ICollection<SubMedServiceDoctor> SubMedServiceDoctors { get; set; }
    }
}
