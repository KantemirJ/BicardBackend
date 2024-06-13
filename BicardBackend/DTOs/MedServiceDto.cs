using System.ComponentModel.DataAnnotations;

namespace BicardBackend.DTOs
{
    public class MedServiceDto
    {
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
