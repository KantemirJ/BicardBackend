using Microsoft.AspNetCore.Identity;

namespace BicardBackend.Models
{
    public class User : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public DateTime BirthDay { get; set; }
        public string? Sex { get; set; }
        public string? PhotoPath { get; set; } = "PhotosOfUsers\\DefaultAvatar.png";
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
