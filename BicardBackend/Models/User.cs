using Microsoft.AspNetCore.Identity;

namespace BicardBackend.Models
{
    public class User : IdentityUser<int>
    {
        public string? PhotoPath { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
