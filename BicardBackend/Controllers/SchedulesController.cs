using BicardBackend.Data;
using BicardBackend.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetEmployeeScheduleById/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var list = await _context.DayOfWeeks.Where(d => d.EmployeeId == id).ToListAsync();
            return Ok(list);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(DayOfWeekDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state");
            }
            Models.DayOfWeek day = new()
            {
                Name = dto.Name,
                StartTime = dto.StartTime,
                EmployeeId = dto.EmployeeId,
                EndTime = dto.EndTime
            };
            _context.DayOfWeeks.Add(day);
            await _context.SaveChangesAsync();
            return Ok("Created.");
        }
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, DayOfWeekDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state");
            }
            var day = _context.DayOfWeeks.SingleOrDefault(d => d.Id == id);
            if (day == null)
            {
                return NotFound();
            }
            day.Name = dto.Name;
            day.StartTime = dto.StartTime;
            day.EndTime = dto.EndTime;
            day.EmployeeId = dto.EmployeeId;
            await _context.SaveChangesAsync();
            return Ok("Updated");
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var day = _context.DayOfWeeks.SingleOrDefault(d =>d.Id == id);
            if (day == null)
            {
                return NotFound();
            }
            _context.DayOfWeeks.Remove(day);
            await _context.SaveChangesAsync();
            return Ok("Deleted.");
        }
    }
}
