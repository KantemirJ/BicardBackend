using BicardBackend.Data;
using BicardBackend.Models;
using BicardBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacanciesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITgBotService _tgBotService;
        public VacanciesController(ApplicationDbContext context, ITgBotService tgBotService)
        {
            _context = context;
            _tgBotService = tgBotService;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var listOfVacancies = await _context.Vacancies.ToListAsync();
            return Ok(listOfVacancies);
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var vacancy = await _context.Vacancies.Where(a => a.Id == id).FirstOrDefaultAsync();
            return Ok(vacancy);
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
            _tgBotService.SendMessageAsync($"A new vacancy is created. Position: {newVacancy.Position}");
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
