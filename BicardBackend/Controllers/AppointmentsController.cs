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
        public async Task<IActionResult> Create(AppointmentDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var subMedService = await _context.Subs.FindAsync(model.SubMedServiceId);
            if (subMedService == null)
            {
                return NotFound();
            }
            try
            {
                var appointment = new Appointment()
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    SubMedService = subMedService,
                    Age = model.Age,
                    TimeAtSchedule = model.TimeAtSchedule
                };
                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Exception message:", ex.Message);
                return StatusCode(500);
            }
        }
        [HttpGet("GetListOfAppointments")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetListOfAppointments()
        {
            var list = await _context.Appointments.ToListAsync();
            return Ok(list);
        }
        [HttpPut("ConfirmAppointment/{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmAppointment(int id, AppointmentDto model)
        {
            var appointment = await _context.Appointments.SingleOrDefaultAsync(x => x.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
            var subMedService = await _context.Subs.FindAsync(model.SubMedServiceId);
            if (subMedService == null)
            {
                return NotFound();
            }
            appointment.Name = model.Name;
            appointment.Email = model.Email;
            appointment.PhoneNumber = model.PhoneNumber;
            appointment.Age = model.Age;
            appointment.TimeAtSchedule = model.TimeAtSchedule;
            appointment.SubMedService = subMedService;
            appointment.IsConfirmed = true;
            await _context.SaveChangesAsync();
            return Ok("Appointment confirmed successfully.");
        }
        [HttpDelete("Cancel")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = new Appointment { Id = id };
            _context.Appointments.Attach(appointment);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return Ok("Canceled.");
        }
    }
}
