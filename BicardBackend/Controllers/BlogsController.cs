
using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using BicardBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public BlogsController(ApplicationDbContext context, IFileService fileService, UserManager<User> userManager)
        {
            _fileService = fileService;
            _context = context;
            _userManager = userManager;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int pageSize, int pageNumber)
        {
            var listOfBlogs = _context.Blogs
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(listOfBlogs);
        }
        [HttpGet("GetLatest")]
        public async Task<IActionResult> GetLatest()
        {
            var listOfBlogs = _context.Blogs
                .OrderByDescending(a => a.Timestamp)
                .Take(3)
                .ToList();
            return Ok(listOfBlogs);
        }
        [HttpGet("GetLatestByDoctor")]
        public async Task<IActionResult> GetLatestByDoctor(int doctorId)
        {
            var listOfBlogs = _context.Blogs
                .Where(a => a.AuthorId == doctorId)
                .OrderByDescending(a => a.Timestamp)
                .Take(3)
                .ToList();
            return Ok(listOfBlogs);
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            return Ok(blog);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] BlogDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }
            
            Blog newBlog = new()
            {
                Title = dto.Title,
                Text = dto.Text,
                AuthorId = dto.AuthorId,
                PhotoPath = await _fileService.SaveFileAsync(dto.Photo, "PhotosOfBlogs"),
            };
            _context.Blogs.Add(newBlog);
            await _context.SaveChangesAsync();
            return Ok(newBlog);
        }
        [HttpPut("Update")]
        public async Task<IActionResult> Update(int id, [FromForm] BlogDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }
            var blog = _context.Blogs.Find(id);
            if (blog == null)
            {
                return NotFound();
            }
            
            blog.Title = dto.Title;
            blog.Text = dto.Text;
            blog.Timestamp = DateTime.UtcNow;
            blog.AuthorId = dto.AuthorId;
            blog.PhotoPath = await _fileService.SaveFileAsync(dto.Photo, "PhotosOfBlogs");
            await _context.SaveChangesAsync();
            return Ok(blog);
        }
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = _context.Blogs.Find(id);
            if (blog == null)
            {
                return BadRequest();
            }
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
            return Ok(blog);
        }
    }
}
