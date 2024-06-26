﻿namespace BicardBackend.DTOs
{
    public class AppointmentDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Age { get; set; }
        public DateTime Date { get; set; }
        public int DoctorId { get; set; }
        public int? UserId { get; set; }
    }
}
