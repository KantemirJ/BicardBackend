using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using BicardBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;
        public ArticlesController(IFileService fileService, ApplicationDbContext context)
        {
            _fileService = fileService;
            _context = context;
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                article.Id,
                article.Title,
                article.FilePath,
                article.AuthorName,
                article.Timestamp
            });
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Articles.Select(a => new {a.Id, a.Title, a.FilePath, a.AuthorName, a.Timestamp}).ToListAsync();
            return Ok(list);
        }
        [HttpGet("GetByAuthorName")]
        public async Task<IActionResult> GetByAuthorName(string name)
        {
            var list = await _context.Articles.Where(a => a.AuthorName == name).Select(a => new { a.Id, a.Title, a.FilePath, a.AuthorName, a.Timestamp }).ToListAsync();
            return Ok(list);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm]ArticleDto dto)
        {
            Article newArticle = new()
            {
                Title = dto.Title,
                FilePath = await _fileService.SaveFileAsync(dto.File, "Articles"),
                AuthorName = dto.AuthorName
            };
            _context.Articles.Add(newArticle);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(int id, ArticleDto dto)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return BadRequest();
            }
            article.Title = dto.Title;
            article.AuthorName = dto.AuthorName;
            _fileService.DeleteFile(article.FilePath);
            article.FilePath = await _fileService.SaveFileAsync(dto.File, "Articles");
            await _context.SaveChangesAsync();
            return Ok(article);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if(article == null)
            {
                return BadRequest();
            }
            _fileService.DeleteFile(article.FilePath);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return Ok(article);
        }
    }
}
