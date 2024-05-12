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
            var groupedFaqs = _context.Faqs.GroupBy(f => f.Type)
                      .Select(group => new
                      {
                          type = group.Key,
                          faqs = group.ToArray()
                      });

            return Ok(groupedFaqs);
        }
        [HttpGet("GetByType")]
        public async Task<IActionResult> Get(string type)
        {
            var listOfFaqs = await _context.Faqs.Where(a => a.Type.Contains(type)).ToListAsync();

            return Ok(listOfFaqs);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] Faq faq)
        {
            _context.Faqs.Add(faq);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] Faq model)
        {
            var faq = await _context.Faqs.FindAsync(model.Id);
            if (faq == null)
            {
                return BadRequest("Not Found.");
            }
            try
            {
                faq.Answer = model.Answer;
                faq.Question = model.Question;
                faq.Type = model.Type;
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
