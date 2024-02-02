using AutoMapper;
using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DoctorsController(ApplicationDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("GetListOfDoctors")]
        public async Task<IActionResult> GetListOfDoctors()
        {
            var listOfDoctors = await _context.Doctors.ToListAsync();
            return Ok(listOfDoctors);
        }
        [HttpGet("GetDoctorById")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }
        [HttpPost("Create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromBody] DoctorDto doctorDto)
        {
            if (doctorDto == null)
            {
                return BadRequest("Invalid data");
            }
            var doctor = _mapper.Map<Doctor>(doctorDto);
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return Ok("Doctor created successfully");
        }
        [HttpPut("Update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromBody] DoctorDto doctorDto)
        {
            var doctor = _mapper.Map<Doctor>(doctorDto);
            _context.Entry(doctor).State = EntityState.Modified;
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
    }
}
