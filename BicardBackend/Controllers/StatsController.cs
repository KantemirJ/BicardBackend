using BicardBackend.Data;
using BicardBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get() 
        {
            var stats = await _context.Clinicstats.FirstOrDefaultAsync();
            return Ok(stats);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ClinicStats model)
        {
            if (ModelState.IsValid)
            {
                await _context.Clinicstats.AddAsync(model);
                await _context.SaveChangesAsync();
            }
            return BadRequest("Invalid input.");
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(ClinicStats model)
        {
            var stats = await _context.Clinicstats.FirstOrDefaultAsync(s => s.Id == model.Id);
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
            var stats = await _context.Clinicstats.FirstOrDefaultAsync(s => s.Id == id);
            if (stats == null)
            {
                return BadRequest("Not Fount");
            }
            _context.Clinicstats.Remove(stats);
            await _context.SaveChangesAsync();
            return Ok(stats);
        }
    }
}
