using Microsoft.AspNetCore.Identity;

namespace BicardBackend.Models
{
    public class User : IdentityUser<int>
    {
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
