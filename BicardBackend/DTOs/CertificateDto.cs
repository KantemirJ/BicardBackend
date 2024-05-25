namespace BicardBackend.DTOs
{
    public class CertificateDto
    {
        public int Id { get; set; }
        public IFormFile Photo { get; set; }
        public string Description { get; set; }
    }
}
