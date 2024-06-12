namespace BicardBackend.DTOs
{
    public class AboutClinicDto
    {
        public string Intro { get; set; }
        public IFormFile? Photo { get; set; }
        public int NumberOfBeds { get; set; }
        public int NumberOfPatients { get; set; }
        public int NumberOfEmployees { get; set; }
    }
}
