using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public InfosController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get()
        {
            var list = await _context.Info.ToListAsync();
            return Ok(list);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] InfoDto model)
        {
            Info newInfo = new()
            {
                Question = model.Question,
                Answer = model.Answer,
                Type = model.Type
            };

            _context.Add(newInfo);
            await _context.SaveChangesAsync();
            return Ok(newInfo);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] InfoDto model)
        {
            var info = _context.Info.Find(model.Id);
            if (info == null)
            {
                return BadRequest();
            }
            info.Question = model.Question;
            info.Answer = model.Answer;
            info.Type = model.Type;
            await _context.SaveChangesAsync();
            return Ok(info);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var info = _context.Info.Find(id);
            if (info == null)
            {
                return BadRequest();
            }
            _context.Info.Remove(info);
            await _context.SaveChangesAsync();
            return Ok(info);
        }
    }
}
