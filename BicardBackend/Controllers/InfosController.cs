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
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Info.ToListAsync();
            return Ok(list);
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var info = await _context.Info.Where(a => a.Id == id).FirstOrDefaultAsync();
            return Ok(info);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(InfoDto model)
        {
            Info newInfo = new()
            {
                Title = model.Title,
                Content = model.Content
            };

            _context.Add(newInfo);
            await _context.SaveChangesAsync();
            return Ok(newInfo);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(int id, InfoDto model)
        {
            var info = _context.Info.Find(id);
            if (info == null)
            {
                return BadRequest();
            }
            info.Title = model.Title;
            info.Content = model.Content;
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
