﻿
using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using BicardBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;
        public BlogsController(ApplicationDbContext context, IFileService fileService)
        {
            _fileService = fileService;
            _context = context;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int pageSize, int pageNumber)
        {
            var listOfBlogs = _context.Blogs
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            foreach (var item in listOfBlogs)
            {
                item.PhotoPath = await _fileService.ConvertFileToBase64(item.PhotoPath);
            }
            return Ok(listOfBlogs);
        }
        [HttpGet("GetLatest")]
        public async Task<IActionResult> GetLatest()
        {
            var listOfBlogs = _context.Blogs
                .OrderByDescending(a => a.Timestamp)
                .Take(3)
                .ToList();
            foreach (var item in listOfBlogs)
            {
                item.PhotoPath = await _fileService.ConvertFileToBase64(item.PhotoPath);
            }
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
            foreach (var item in listOfBlogs)
            {
                item.PhotoPath = await _fileService.ConvertFileToBase64(item.PhotoPath);
            }
            return Ok(listOfBlogs);
        }
        [HttpGet("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            blog.PhotoPath = await _fileService.ConvertFileToBase64(blog.PhotoPath);
            return Ok(blog);
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] BlogDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }
            var author = _context.Doctors.Find(dto.AuthorId);
            if (author == null)
            {
                return BadRequest("Doctor not found.");
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
            var blog = _context.Blogs.Find(dto.Id);
            if (blog == null)
            {
                return NotFound();
            }
            var author = _context.Doctors.Find(dto.AuthorId);
            if(author == null)
            {
                return BadRequest();
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
