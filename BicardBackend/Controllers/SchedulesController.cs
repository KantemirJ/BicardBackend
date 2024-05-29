using Bicard.Controllers;
using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
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
        private readonly ApplicationDbContext _context;

        public SchedulesController(ApplicationDbContext context, ILogger<AppointmentsController> logger)
        {
            _context = context;
        }
        [HttpGet("GetByDoctorId")]
        public async Task<IActionResult> GetByDoctorId(int id)
        {
            var schedule = _context.Schedules.Where(a => a.DoctorId == id).ToList();
            return Ok(schedule);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(ScheduleDto dto)
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
        [HttpPost("Update")]
        public async Task<IActionResult> Update(ScheduleDto dto)
        {
            var schedule = _context.Schedules.FirstOrDefault(a => a.Id == dto.Id);
            if (schedule == null)
            {
                return BadRequest("Not Found");
            }
            schedule.DayOfWeek = dto.DayOfWeek;
            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;
            schedule.DoctorId = dto.DoctorId;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = _context.Schedules.FirstOrDefault(a => a.Id == id);
            if (schedule == null)
            {
                return BadRequest("Not Found");
            }
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("GetTimetable")]
        public async Task<IActionResult> GetTimetable()
        {
            List<TimeTable> timeTables = new();
            var listOfDoctorId = _context.Doctors.Select(a => new { a.Id, a.Name, a.Speciality }).ToList();
            var listOfSchedules = _context.Schedules.ToList();
            foreach (var doctor in listOfDoctorId)
            {
                TimeTable timeTable = new()
                {
                    DoctorId = doctor.Id,
                    DoctorName = doctor.Name,
                    DoctorSpecialty = doctor.Speciality
                };
                for (int i = 0; i < 7; i++)
                {
                    var currentDay = DateTime.Today.AddDays(i).ToUniversalTime();
                    var dayOfWeek = GetRussianDayOfWeekName(currentDay.DayOfWeek);
                    TimeTableDay timeTableDay = new()
                    {
                        Date = currentDay,
                        DayOfWeek = dayOfWeek,
                        StartTime = listOfSchedules
                        .Where(a => a.DayOfWeek == currentDay.DayOfWeek && a.DoctorId == doctor.Id)
                        .Select(a => a.StartTime)
                        .FirstOrDefault() ?? "Null",
                        EndTime = listOfSchedules
                        .Where(a => a.DayOfWeek == currentDay.DayOfWeek && a.DoctorId == doctor.Id)
                        .Select(a => a.EndTime)
                        .FirstOrDefault() ?? "Null",
                    };
                    timeTable.Days.Add(timeTableDay);
                }
                timeTables.Add(timeTable);
            }


            return Ok(timeTables);
        }
        [HttpPost("GetTimeSlots")]
        public async Task<IActionResult> GetTimeSlots(DateTime currentDay, int doctorId)
        {
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
            var timeslots = GenerateTimeSlotsWithStatus(schedule?.StartTime ?? null, schedule?.EndTime ?? null, 30, appointments);
            return Ok(new
            {
                Date = currentDay,
                dayOfWeek,
                startTime = schedule?.StartTime ?? null, // Handle null schedule
                endTime = schedule?.EndTime ?? null,
                timeslots
            });
        }

        [HttpPost("GetTimetableByDoctor")]
        public async Task<IActionResult> GetTimetableByDoctor(DateTime day, int doctorId)
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

        private string GetRussianDayOfWeekName(DayOfWeek dayOfWeek)
        {
            return DayOfWeekNamesRu.ContainsKey(dayOfWeek) ? DayOfWeekNamesRu[dayOfWeek] : "Неизвестный день";
        }
        public static List<Dictionary<string, string>> GenerateTimeSlotsWithStatus(string startTime, string endTime, int timeInterval = 30, List<string> appointments = null)
        {
            try
            {
                // Existing code for parsing start and end times and error handling...

                List<Dictionary<string, string>> timeSlots = new List<Dictionary<string, string>>();
                TimeSpan startTimeSpan = TimeSpan.Parse(startTime);
                TimeSpan endTimeSpan = TimeSpan.Parse(endTime);

                TimeSpan currentTime = startTimeSpan;
                while (currentTime < endTimeSpan)
                {
                    TimeSpan nextTime = currentTime.Add(TimeSpan.FromMinutes(timeInterval));
                    string timeSlot = $"{currentTime:hh\\:mm}";
                    string status = appointments.Contains(timeSlot) ? "booked" : "available"; // Null-coalescing for appointments list
                    timeSlots.Add(new Dictionary<string, string>() { { "Time", timeSlot }, { "Status", status } });
                    currentTime = nextTime;
                }

                return timeSlots;
            }
            catch (FormatException)
            {
                throw new ArgumentException("Start time and end time must be in HH:MM format.");
            }
        }
    }
}
