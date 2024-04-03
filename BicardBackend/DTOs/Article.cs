namespace BicardBackend.DTOs
{
    public class ArticleDto
    {
        public string Title { get; set; }
        public IFormFile File { get; set; }
        public string AuthorName { get; set; }
    }
}
