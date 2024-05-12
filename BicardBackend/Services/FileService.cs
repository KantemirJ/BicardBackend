using Microsoft.Extensions.FileProviders;

namespace BicardBackend.Services
{
    public class FileService : IFileService
    {

        public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            // Define the folder where you want to store the files
            string uploadsFolder = "C:\\BicardUpload";

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

            // Return the full path of the saved file
            return filePath;
        }
        public async Task<string> ConvertFileToBase64(string filePath)
        {
            try
            {
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                string base64String = Convert.ToBase64String(fileBytes);

                return base64String;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting file to base64: {ex.Message}");
                return null;
            }
        }
        public void DeleteFile(string path)
        {
            try
            {
                if (System.IO.File.Exists(Path.Combine(path)))
                {
                    System.IO.File.Delete(Path.Combine(path));
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
