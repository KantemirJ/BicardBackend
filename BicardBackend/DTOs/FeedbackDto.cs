using BicardBackend.Models;
using System.ComponentModel.DataAnnotations;

namespace BicardBackend.DTOs
{
    public class FeedbackDto
    {
        [Required]
        public string Message { get; set; }
        [Required]
        public int UserId { get; set; }
        public int? DoctorId { get; set; }
    }
}
