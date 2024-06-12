using BicardBackend.Data;
using BicardBackend.Models;
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
        public AboutClinicController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get() 
        {
            var stats = await _context.AboutClinic.ToListAsync();
            return Ok(stats);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(AboutClinic model)
        {
            if (ModelState.IsValid)
            {
                await _context.AboutClinic.AddAsync(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            return BadRequest("Invalid input.");
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(AboutClinic model)
        {
            var stats = await _context.AboutClinic.FirstOrDefaultAsync(s => s.Id == model.Id);
            if (stats == null)
            {
                return BadRequest("Not Fount");
            }
            stats.NumberOfEmployees = model.NumberOfEmployees;
            stats.NumberOfBeds = model.NumberOfBeds;
            stats.NumberOfPatients = model.NumberOfPatients;
            await _context.SaveChangesAsync();
            return Ok(stats);
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
