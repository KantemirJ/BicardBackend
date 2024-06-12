using Microsoft.Extensions.FileProviders;
using Telegram.Bot.Types;

namespace BicardBackend.Services
{
    public class FileService : IFileService
    {
        private readonly string uploadsFolder = "C:\\Temp";
        public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            
            // Ensure the "Uploads" folder exists; create it if it doesn't
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Create a subfolder with the current date (e.g., "Uploads/2022-01-01")
            string dateSubfolder = Path.Combine(uploadsFolder, subFolder);
            if (!Directory.Exists(dateSubfolder))
            {
                Directory.CreateDirectory(dateSubfolder);
            }

            // Generate a unique file name
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(dateSubfolder, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var tempPath = Path.Combine(subFolder, fileName);

            return tempPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        public void DeleteFile(string path)
        {
            var fullPath = Path.Combine(uploadsFolder, path);
            try
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    Console.WriteLine("File deleted.");
                }
                else Console.WriteLine("File not found");
            }
            catch (IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }
        }
    }

}
