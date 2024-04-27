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
        private static readonly Dictionary<DayOfWeek, string> DayOfWeekNamesRu =
            new Dictionary<DayOfWeek, string>()
            {
              { DayOfWeek.Sunday, "Воскресенье" },
              { DayOfWeek.Monday, "Понедельник" },
              { DayOfWeek.Tuesday, "Вторник" },
              { DayOfWeek.Wednesday, "Среда" },
              { DayOfWeek.Thursday, "Четверг" },
              { DayOfWeek.Friday, "Пятница" },
              { DayOfWeek.Saturday, "Суббота" },
            };
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
        public async Task<IActionResult> GetListOfAppointments(int doctorId)
        {
            var list = await _context.Appointments.Where(a => a.DoctorId == doctorId).ToListAsync();
            return Ok(list);
        }
        [HttpGet("GetUnconfirmedAppointments")]
        public async Task<IActionResult> GetUnconfirmedAppointments()
        {
            var list = await _context.Appointments.Where(a => a.IsConfirmed == false).ToListAsync();
            return Ok(list);
        }
        [HttpPost("GetTimetable")]
        public async Task<IActionResult> GetTimetable(DateTime day, int doctorId)
        {
            // List to store time slots for multiple days
            List<object> timeSlots = new List<object>();

            for (int i = 0; i < 7; i++)
            {
                var currentDay = day.AddDays(i);
                var dayOfWeek = GetRussianDayOfWeekName(currentDay.DayOfWeek);

                // Get schedule for current day
                var schedule = _context.Schedules
                    .Where(a => a.DayOfWeek == currentDay.DayOfWeek && a.DoctorId == doctorId)
                    .FirstOrDefault();

                // Get appointments for current day
                var appointments = _context.Appointments
                    .Where(a => a.Date.Date == currentDay.Date && a.DoctorId == doctorId)
                    .Select(a => a.Date.ToString("HH:mm"))
                    .ToList();

                // Add time slot object for current day
                timeSlots.Add(new
                {
                    Date = currentDay,
                    dayOfWeek,
                    startTime = schedule?.StartTime ?? null, // Handle null schedule
                    endTime = schedule?.EndTime ?? null,
                    appointments
                });
            }

            return Ok(timeSlots);
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
        [HttpPost("CreateSchedule")]
        public async Task<IActionResult> CreateSchedule(ScheduleDto dto)
        {
            Schedule schedule = new() 
            {
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                DoctorId = dto.DoctorId
            };
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return Ok();
        }
        private string GetRussianDayOfWeekName(DayOfWeek dayOfWeek)
        {
            return DayOfWeekNamesRu.ContainsKey(dayOfWeek) ? DayOfWeekNamesRu[dayOfWeek] : "Неизвестный день";
        }
    }
}
