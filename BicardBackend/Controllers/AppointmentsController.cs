using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using BicardBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Telegram.Bot.Types;

namespace Bicard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IEmailService _emailService;
        
        public AppointmentsController(ApplicationDbContext context, ILogger<AppointmentsController> logger, IEmailService emailService) 
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
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
                    UserId = model.UserId
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
        [HttpGet("GetAppointmentsByUserId")]
        public async Task<IActionResult> GetAppointmentsByUserId(int id)
        {
            var list = await _context.Appointments.Where(a => a.UserId == id).ToListAsync();
            List<UserAppointment> response = new List<UserAppointment>();
            foreach (var item in list)
            {
                UserAppointment userAppointment = new UserAppointment();
                var doctor = _context.Doctors.FirstOrDefault(a => a.Id == item.DoctorId);
                if(doctor == null)
                {
                    return BadRequest("Doctor not found.");
                }
                userAppointment.DoctorPhoto = doctor.PathToPhoto;
                userAppointment.DoctorName = doctor.Name;
                userAppointment.DoctorSpeciality = doctor.Speciality;
                userAppointment.AppointmentDate = item.Date.ToString("dd.MM.yyyy HH:mm");
                userAppointment.IsConfirmed = item.IsConfirmed;
                response.Add(userAppointment);
            }

            return Ok(response);
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
            var doctor = _context.Doctors.SingleOrDefault(x => x.Id == appointment.DoctorId);
            if (doctor == null)
            {
                return BadRequest("Doctor not found.");
            }
            appointment.Name = model.Name;
            appointment.Email = model.Email;
            appointment.PhoneNumber = model.PhoneNumber;
            appointment.Age = model.Age;
            appointment.Date = model.Date;
            appointment.DoctorId = model.DoctorId;
            appointment.IsConfirmed = true;
            await _context.SaveChangesAsync();
            var template = await _emailService.ReadTemplateFileAsync("BicardBackend.EmailTemplates.AppointmentConfirmation.html");
            template = template.Replace("[Patient Name]", appointment.Name);
            template = template.Replace("[Doctor Name]", doctor.Name);
            template = template.Replace("[Date]", appointment.Date.ToString("dd.MM.yyyy"));
            template = template.Replace("[Time]", appointment.Date.ToString("HH:mm"));
            _emailService.Send(appointment.Email, "Статус встречи", template);
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
