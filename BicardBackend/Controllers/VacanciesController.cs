using BicardBackend.Data;
using BicardBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacanciesController : ControllerBase
    {
        private ApplicationDbContext _context;
        public VacanciesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            var listOfVacancies = await _context.Vacancies.ToListAsync();
            return Ok(listOfVacancies);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(VacancyDto dto)
        {
            Vacancy newVacancy = new()
            {
                Position = dto.Position,
                Requirements = dto.Requirements,
                Description = dto.Description
            };
            _context.Vacancies.Add(newVacancy);
            await _context.SaveChangesAsync();
            return Ok(newVacancy);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(int id, VacancyDto dto)
        {
            var vacancy = _context.Vacancies.SingleOrDefault(x => x.Id == id);
            if (vacancy == null)
            {
                return BadRequest("Not Found.");
            }
            vacancy.Position = dto.Position;
            vacancy.Requirements = dto.Requirements;
            vacancy.Description = dto.Description;
            await _context.SaveChangesAsync();
            return Ok(vacancy);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var vacancy = _context.Vacancies.SingleOrDefault(x => x.Id == id);
            if (vacancy == null)
            {
                return BadRequest("Not Found.");
            }
            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();
            return Ok(vacancy);
        }
    }
}
