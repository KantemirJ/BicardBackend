namespace BicardBackend.Models
{
    public class MedService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string PathToPhoto { get; set; }
        public ICollection<SubMedService> SubMedServices { get; set; }

    }
}
