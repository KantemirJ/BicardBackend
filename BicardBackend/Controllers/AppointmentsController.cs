using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bicard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;
        public AppointmentsController(ApplicationDbContext context, ILogger<AppointmentsController> logger) 
        {
            _context = context;
            _logger = logger;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(Appointment model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                await _context.Appointments.AddAsync(model);
                await _context.SaveChangesAsync();
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Create appointment exeption: {FirstName}", ex.Message);
                return StatusCode(500);
            }
        }
        [HttpGet("GetListOfAppointments")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetListOfAppointments()
        {
            var list = await _context.Appointments.ToListAsync();
            return Ok(list);
        }
        [HttpPut("ConfirmAppointment")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmAppointment(AppointmentConfirmation model)
        {
            var appointment = await _context.Appointments.SingleOrDefaultAsync(x => x.Id == model.Id);
            if (appointment == null)
            {
                return NotFound();
            }
            appointment.IsConfirmed = true;
            appointment.TimeAtSchedule = model.TimeAtSchedule;
            appointment.ConfirmedByUserId = model.ConfirmedByUserId;
            await _context.SaveChangesAsync();
            return Ok("Appointment confirmed successfully.");
        }
        [HttpDelete("Cancel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = new Appointment { Id = id
            };
            _context.Appointments.Attach(appointment);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return Ok("Canceled.");
        }
    }
}
