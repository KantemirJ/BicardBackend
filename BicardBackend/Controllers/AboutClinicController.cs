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
    public class AboutClinicController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        public AboutClinicController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get() 
        {
            var stats = await _context.AboutClinic.ToListAsync();
            return Ok(stats);
        }
        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(int id)
        {
            var stats = await _context.AboutClinic.Where(a => a.Id == id).FirstOrDefaultAsync();
            return Ok(stats);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] AboutClinicDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input.");
            }
            AboutClinic aboutClinic = new()
            {
                Intro = model.Intro,
                PathToPhoto = await _fileService.SaveFileAsync(model.Photo, "AboutClinic"),
                NumberOfBeds = model.NumberOfBeds,
                NumberOfEmployees = model.NumberOfEmployees,
                NumberOfPatients = model.NumberOfPatients
            };
            _context.AboutClinic.Add(aboutClinic);
            await _context.SaveChangesAsync();
            return Ok(aboutClinic);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromQuery] int id,[FromForm] AboutClinicDto model)
        {
            var aboutClinic = await _context.AboutClinic.FirstOrDefaultAsync(s => s.Id == id);
            if (aboutClinic == null)
            {
                return BadRequest("Not Fount");
            }
            aboutClinic.Intro = model.Intro;
            aboutClinic.NumberOfEmployees = model.NumberOfEmployees;
            aboutClinic.NumberOfBeds = model.NumberOfBeds;
            aboutClinic.NumberOfPatients = model.NumberOfPatients;
            if (model.Photo != null)
            {
                try 
                {
                    _fileService.DeleteFile(aboutClinic.PathToPhoto);
                    aboutClinic.PathToPhoto = await _fileService.SaveFileAsync(model.Photo, "AboutClinic");
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            await _context.SaveChangesAsync();
            return Ok(aboutClinic);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var stats = await _context.AboutClinic.FirstOrDefaultAsync(s => s.Id == id);
            if (stats == null)
            {
                return BadRequest("Not Fount");
            }
            _context.AboutClinic.Remove(stats);
            await _context.SaveChangesAsync();
            return Ok(stats);
        }
    }
}
