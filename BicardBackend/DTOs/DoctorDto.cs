namespace BicardBackend.DTOs
{
    public class DoctorDto
    {
        public string? Name { get; set; }
        public string? Speciality { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public IFormFile? Photo { get; set; }
        public int UserId { get; set; }
    }
}
