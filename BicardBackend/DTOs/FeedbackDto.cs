using BicardBackend.Models;
using System.ComponentModel.DataAnnotations;

namespace BicardBackend.DTOs
{
    public class FeedbackDto
    {
        [Required]
        public string Message { get; set; }
    }
}
