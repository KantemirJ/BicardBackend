using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using BicardBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        public CertificatesController(ApplicationDbContext context, IFileService fileService) 
        {
            _context = context;
            _fileService = fileService;
        } 
        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var cert = await _context.Certificates.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(cert);
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var cert = await _context.Certificates.ToListAsync();
            return Ok(cert);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm]CertificateDto model)
        {
            Certificate certificate = new Certificate
            {
                PhotoPath = await _fileService.SaveFileAsync(model.Photo, "Certificates"),
                Description = model.Description
            };
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();
            return Ok(certificate);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm]CertificateDto model)
        {
            var cert = _context.Certificates.FirstOrDefault(a => a.Id == model.Id);
            if (cert == null)
            {
                return BadRequest("Not Found.");
            }
            _fileService.DeleteFile(cert.PhotoPath);
            cert.PhotoPath = await _fileService.SaveFileAsync(model.Photo, "Certificates");
            cert.Description = model.Description;
            
            await _context.SaveChangesAsync();
            return Ok(cert);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var cert = _context.Certificates.FirstOrDefault(a => a.Id == id);
            if (cert == null)
            {
                return BadRequest("Not Found.");
            }
            _fileService.DeleteFile(cert.PhotoPath);
            _context.Certificates.Remove(cert);

            await _context.SaveChangesAsync();
            return Ok(cert);
        }
    }
}
