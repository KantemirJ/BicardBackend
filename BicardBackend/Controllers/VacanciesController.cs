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
        private readonly IFileService _fileService;
        public VacanciesController(ApplicationDbContext context, ITgBotService tgBotService, IFileService fileService)
        {
            _context = context;
            _tgBotService = tgBotService;
            _fileService = fileService;
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
        [HttpPost("Response")]
        public async Task<IActionResult> Response([FromQuery]int vacancyId, IFormFile file)
        {
            var vacancy = _context.Vacancies.FirstOrDefault(v => v.Id == vacancyId);
            if (vacancy == null)
            {
                return BadRequest("Not Found.");
            }
            try
            {
                var pathToFile = await _fileService.SaveFileAsync(file, "ResponsesToVacancies");
                await _tgBotService.SendMessageAsync($"Отклик на позицию \"{vacancy.Position}\"");
                await _tgBotService.SendPdfAsync(Path.Combine("C:\\Temp", pathToFile));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
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
