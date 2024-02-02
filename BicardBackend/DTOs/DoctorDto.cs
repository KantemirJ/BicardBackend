namespace BicardBackend.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Speciality { get; set; }
        public string? Bio { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public IFormFile? Photo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int UserId { get; set; }
    }
}
