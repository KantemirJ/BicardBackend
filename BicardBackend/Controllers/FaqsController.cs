using BicardBackend.Data;
using BicardBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaqsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public FaqsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            var listOfFaqs = await _context.Faqs.ToListAsync();

            return Ok(listOfFaqs);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(string question, string answer)
        {
            Faq newFaq = new()
            {
                Question = question,
                Answer = answer
            };
            _context.Faqs.Add(newFaq);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(int id, string question, string answer)
        {
            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null)
            {
                return BadRequest("Not Found.");
            }
            try
            {
                faq.Answer = answer;
                faq.Question = question;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null)
            {
                return BadRequest("Not Found.");
            }
            _context.Faqs.Remove(faq);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
