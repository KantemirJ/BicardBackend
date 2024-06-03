﻿using Microsoft.AspNetCore.Identity;

namespace BicardBackend.Models
{
    public class User : IdentityUser<int>
    {
        public string? BirthDay { get; set; }
        public string? Sex { get; set; }
        public string? PhotoPath { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
