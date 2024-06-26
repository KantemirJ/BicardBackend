﻿using AutoMapper;
using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using BicardBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        //private readonly IMapper _mapper;
        private readonly IFileService _fileService;
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
        public DoctorsController(ApplicationDbContext context, IMapper mapper, IFileService fileService, UserManager<User> userManager, RoleManager<Role> roleManager) 
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileService;
            _roleManager = roleManager;
        }
        [HttpGet("GetListOfDoctors")]
        public async Task<IActionResult> GetListOfDoctors()
        {
            var listOfDoctors = await _context.Doctors.ToListAsync();

            var listOfDoctorsDto = listOfDoctors.Select(async doctor =>
            {
                return new
                {
                    doctor.Id,
                    doctor.Name,
                    doctor.Speciality,
                    doctor.Education,
                    doctor.Experience,
                    doctor.PathToPhoto
                };
            });

            // Wait for all async operations to complete
            var result = await Task.WhenAll(listOfDoctorsDto);

            return Ok(result);
        }
        [HttpGet("GetDoctorById")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            var model = new 
            {
                Id = id,
                doctor.Name,
                doctor.Speciality,
                doctor.Education,
                doctor.Experience,
                doctor.PathToPhoto,
                doctor.UserId,
            };  

            return Ok(model);
        }
        [HttpPost("Create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] DoctorDto doctorDto)
        {
            if (doctorDto == null)
            {
                return BadRequest("Invalid data");
            }
            var user = await _userManager.FindByIdAsync(doctorDto.UserId.ToString());
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var roleExists = await _roleManager.RoleExistsAsync("Doctor");
            if (!roleExists)
            {
                var role = new Role("Doctor");
                await _roleManager.CreateAsync(role);
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles != null)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());
                await _userManager.AddToRoleAsync(user, "Doctor");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Doctor");
            }
            
            Doctor doctor = new() 
            {
                Name = doctorDto.Name,
                Speciality = doctorDto.Speciality,
                Education = doctorDto.Education,
                Experience = doctorDto.Experience,
                PathToPhoto =  await _fileService.SaveFileAsync(doctorDto.Photo, "PhotosOfDoctors"),
                UserId = doctorDto.UserId,
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return Ok("Doctor created successfully");
        }
        [HttpPut("Update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] DoctorDto doctorDto, int id)
        {
            var doctor = _context.Doctors.FirstOrDefault(x => x.Id == id);
            if (doctor == null)
            {
                return BadRequest("Not found.");
            }
            doctor.Name = doctorDto.Name;
            doctor.Speciality = doctorDto.Speciality;
            doctor.Education = doctorDto.Education;
            doctor.Experience = doctorDto.Experience;
            doctor.UserId = doctorDto.UserId;
            if(doctorDto.Photo != null)
            {
                _fileService.DeleteFile(doctor.PathToPhoto);
                doctor.PathToPhoto = await _fileService.SaveFileAsync(doctorDto.Photo, "PhotosOfDoctors");
            }
            await _context.SaveChangesAsync();
            return Ok("Doctor updated successfully");
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return Ok("Doctor deleted successfully");
        }

        [HttpGet("GetSpeciality")]
        public async Task<IActionResult> GetSpeciality()
        {
            var list = _context.Doctors
                .Select(a => a.Speciality)
                .Distinct()
                .ToList();
            return Ok(list);
        }
        [HttpGet("SearchByName")]
        public async Task<IActionResult> SearchByName(string name)
        {
            var listOfDoctors = _context.Doctors
                .Where(a => a.Name.Contains(name))
                .ToList();
            var listOfDoctorsDto = listOfDoctors.Select(async doctor =>
            {
                return new
                {
                    doctor.Id,
                    doctor.Name,
                    doctor.Speciality,
                    doctor.Education,
                    doctor.Experience,
                    doctor.PathToPhoto,
                    doctor.UserId
                };
            });

            // Wait for all async operations to complete
            var result = await Task.WhenAll(listOfDoctorsDto);

            return Ok(result);
        }
        [HttpGet("SearchBySpeciality")]
        public async Task<IActionResult> SearchBySpeciality(string speciality)
        {
            var listOfDoctors = _context.Doctors
                .Where(a => a.Speciality.Contains(speciality))
                .ToList();
            var listOfDoctorsDto = listOfDoctors.Select(async doctor =>
            {
                return new
                {
                    doctor.Id,
                    doctor.Name,
                    doctor.Speciality,
                    doctor.Education,
                    doctor.Experience,
                    doctor.PathToPhoto,
                    doctor.UserId
                };
            });

            // Wait for all async operations to complete
            var result = await Task.WhenAll(listOfDoctorsDto);

            return Ok(result);
        }
        [HttpGet("GetWorkingHours")]
        public async Task<IActionResult> GetWorkingHours(int id)
        {
            var schedule = _context.Schedules
                .Where(a => a.DoctorId == id)
                .Select(a => new 
                {
                    a.DayOfWeek, 
                    a.StartTime, 
                    a.EndTime 
                })
                .ToList();
            
            return Ok(schedule);
        }
        public static string GetRussianDayOfWeekName(DayOfWeek dayOfWeek)
        {
            return DayOfWeekNamesRu.ContainsKey(dayOfWeek) ? DayOfWeekNamesRu[dayOfWeek] : "Неизвестный день";
        }
    }
}
