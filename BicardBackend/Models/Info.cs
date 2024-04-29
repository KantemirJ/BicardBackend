﻿namespace BicardBackend.Models
{
    public class Info
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
