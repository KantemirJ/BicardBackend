namespace BicardBackend.Services
{
    public interface IFileService
    {
        public Task<string> SaveFileAsync(IFormFile file, string subFolder);
    }
}
