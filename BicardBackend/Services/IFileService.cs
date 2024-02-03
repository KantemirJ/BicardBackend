namespace BicardBackend.Services
{
    public interface IFileService
    {
        public Task<string> SaveFileAsync(IFormFile file, string subFolder);
        public Task<string> ConvertFileToBase64(string filePath);
    }
}
