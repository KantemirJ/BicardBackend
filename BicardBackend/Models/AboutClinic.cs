namespace BicardBackend.Models
{
    public class AboutClinic
    {
        public int Id { get; set; }
        public string Intro { get; set; }
        public string PathToPhoto { get; set; }
        public int NumberOfBeds { get; set; }
        public int NumberOfPatients { get; set; }
        public int NumberOfEmployees{ get; set; }
    }
}
