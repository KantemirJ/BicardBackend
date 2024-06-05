using BicardBackend.Models;

namespace BicardBackend.DTOs
{
    public class UserProfile
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime BirthDay { get; set; }
        public string? Sex { get; set; }
    }
}
