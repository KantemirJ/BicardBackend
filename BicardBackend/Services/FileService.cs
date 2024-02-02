﻿using Microsoft.Extensions.FileProviders;

namespace BicardBackend.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            // Define the folder where you want to store the files
            string uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads");

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
    }

}
