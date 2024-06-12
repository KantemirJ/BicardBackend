namespace BicardBackend.Services
{
    public interface IFileService
    {
        public Task<string> SaveFileAsync(IFormFile file, string subFolder);
        public void DeleteFile(string path);
    }
}
