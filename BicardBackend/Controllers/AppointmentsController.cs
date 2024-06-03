using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;

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
            try
            {
                var appointment = new Appointment()
                {
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,    
                    Age = model.Age,
                    Date = model.Date,
                    DoctorId = model.DoctorId,
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
            var list = await _context.Appointments.Where(a => a.IsConfirmed == false).ToListAsync();
            return Ok(list);
        }
        [HttpGet("GetConfirmedAppointments")]
        public async Task<IActionResult> GetConfirmedAppointments()
        {
            var list = await _context.Appointments.Where(a => a.IsConfirmed == true).ToListAsync();
            return Ok(list);
        }
        [HttpGet("GetListOfAppointmentsByDoctorId")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetListOfAppointmentsByDoctorId(int id)
        {
            var list = await _context.Appointments.Where(a => a.IsConfirmed == false && a.DoctorId == id).ToListAsync();
            return Ok(list);
        }
        [HttpGet("GetConfirmedAppointmentsByDoctorId")]
        public async Task<IActionResult> GetconfirmedAppointmentsByDoctorId(int id)
        {
            var list = await _context.Appointments.Where(a => a.IsConfirmed == true && a.DoctorId == id).ToListAsync();
            return Ok(list);
        }
        [HttpGet("GetConfirmedAppointmentsByUserId")]
        public async Task<IActionResult> GetConfirmedAppointmentsByUserId(int id)
        {
            var list = await _context.Appointments.Where(a => a.UserId == id).ToListAsync();

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
            appointment.Name = model.Name;
            appointment.Email = model.Email;
            appointment.PhoneNumber = model.PhoneNumber;
            appointment.Age = model.Age;
            appointment.Date = model.Date;
            appointment.DoctorId = model.DoctorId;
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
