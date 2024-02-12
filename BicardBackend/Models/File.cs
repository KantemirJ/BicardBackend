namespace BicardBackend.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Extention { get; set; }
        public double Size { get; set; }
        public int MedServiceId { get; set; }
    }
}
