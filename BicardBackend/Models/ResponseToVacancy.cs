namespace BicardBackend.Models
{
    public class ResponseToVacancy
    {
        public int VacancyId { get; set; }
        public IFormFile CvFile { get; set; }
    }
}
