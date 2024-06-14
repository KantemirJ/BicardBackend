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
            if (model.Photo1 == null || model.Photo2 == null)
            {
                return BadRequest("Invalid input.");
            }
            AboutClinic aboutClinic = new()
            {
                Intro = model.Intro,
                PathToPhoto1 = await _fileService.SaveFileAsync(model.Photo1, "AboutClinic"),
                PathToPhoto2 = await _fileService.SaveFileAsync(model.Photo2, "AboutClinic"),
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
            if (model.Photo1 != null)
            {
                try 
                {
                    _fileService.DeleteFile(aboutClinic.PathToPhoto1);
                    aboutClinic.PathToPhoto1 = await _fileService.SaveFileAsync(model.Photo1, "AboutClinic");
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            if (model.Photo2 != null)
            {
                try
                {
                    _fileService.DeleteFile(aboutClinic.PathToPhoto2);
                    aboutClinic.PathToPhoto2 = await _fileService.SaveFileAsync(model.Photo2, "AboutClinic");
                }
                catch (Exception ex)
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
            var aboutClinic = await _context.AboutClinic.FirstOrDefaultAsync(s => s.Id == id);
            if (aboutClinic == null)
            {
                return BadRequest("Not Fount");
            }
            _fileService.DeleteFile(aboutClinic.PathToPhoto1);
            _fileService.DeleteFile(aboutClinic.PathToPhoto2);
            _context.AboutClinic.Remove(aboutClinic);
            await _context.SaveChangesAsync();
            return Ok(aboutClinic);
        }
    }
}
