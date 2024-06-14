namespace BicardBackend.DTOs
{
    public class AboutClinicDto
    {
        public string Intro { get; set; }
        public IFormFile? Photo1{ get; set; }
        public IFormFile? Photo2 { get; set; }
        public int NumberOfBeds { get; set; }
        public int NumberOfPatients { get; set; }
        public int NumberOfEmployees { get; set; }
    }
}
