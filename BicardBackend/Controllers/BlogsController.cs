
using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using BicardBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
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
            if (blog == null)
            {
                return BadRequest("Not Found.");
            }
            var preNextIds = await GetPreviousNextBlogIds(id);
            return Ok( 
                new
                {
                    previosId = preNextIds.Item1,
                    blog.Id,
                    blog.Title,
                    blog.Text,
                    blog.AuthorId,
                    blog.Timestamp,
                    blog.PhotoPath,
                    nextId = preNextIds.Item2
                }
            );
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
        public async Task<IActionResult> Update([FromForm] BlogDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }
            var blog = _context.Blogs.Find(dto.Id);
            if (blog == null)
            {
                return NotFound();
            }
            
            blog.Title = dto.Title;
            blog.Text = dto.Text;
            blog.Timestamp = DateTime.UtcNow;
            blog.AuthorId = dto.AuthorId;
            if (dto.Photo != null)
            {
                _fileService.DeleteFile(blog.PhotoPath);
                blog.PhotoPath = await _fileService.SaveFileAsync(dto.Photo, "PhotosOfBlogs");
            }
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
        private async Task<Tuple<int?, int?>> GetPreviousNextBlogIds(int id)
        {
            var blogIds = await _context.Blogs
                .OrderBy(b => b.Id)
                .Select(b => b.Id) // Select only Id property
                .ToListAsync();

            int currentIndex = blogIds.IndexOf(id);

            int? previousId = null;
            int? nextId = null;

            if (currentIndex > 0)
            {
                previousId = blogIds[currentIndex - 1];
            }

            if (currentIndex < blogIds.Count - 1)
            {
                nextId = blogIds[currentIndex + 1];
            }

            return new Tuple<int?, int?>(previousId, nextId);
        }
    }
}
