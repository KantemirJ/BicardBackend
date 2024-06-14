﻿namespace BicardBackend.Models
{
    public class AboutClinic
    {
        public int Id { get; set; }
        public string Intro { get; set; }
        public string PathToPhoto1 { get; set; }
        public string PathToPhoto2 { get; set; }
        public int NumberOfBeds { get; set; }
        public int NumberOfPatients { get; set; }
        public int NumberOfEmployees{ get; set; }
    }
}
