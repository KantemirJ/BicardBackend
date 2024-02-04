namespace BicardBackend.Models
{
    public class SubMedServiceDoctor
    {
        public int DoctorId { get; set; }
        public int SubMedServiceId { get; set; }
        public Doctor Doctor { get; set; }
        public SubMedService SubMedService { get; set; }
    }
}
