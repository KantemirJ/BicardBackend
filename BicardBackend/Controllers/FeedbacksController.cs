using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using BicardBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IFileService _fileService;
        public FeedbacksController(ApplicationDbContext context, UserManager<User> userManager, IFileService fileService)
        {
            _userManager = userManager;
            _context = context;
            _fileService = fileService;
        }
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create(FeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            Feedback feedback = new()
            {
                Message = feedbackDto.Message,
                User = user,
            };
            _context.Users.Attach(user);
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
            return Ok("Done.");
        }
        [HttpGet("GetFeedbackById/{id}")]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(feedback.UserId.ToString());
            string path;
            if (user.PhotoPath == null)
            {
                path = "C:\\Users\\user\\source\\repos\\BicardBackend\\BicardBackend\\Uploads\\PhotosOfUsers\\default.png";
            }
            else
            {
                path = user.PhotoPath;
            }
            return Ok(new
            {
                feedback.Id,
                feedback.Message,
                user.UserName,
                UserPhoto = _fileService.ConvertFileToBase64(path).Result,
                timeStamp = feedback.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        
        [HttpGet("GetAllFeedbacks")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var listOfFeedback = await _context.Feedbacks.ToListAsync();
            if (listOfFeedback == null)
            {
                return NotFound();
            }
            var tempList = new List<object>();
            foreach (var feedback in listOfFeedback)
            {
                var user = await _userManager.FindByIdAsync(feedback.UserId.ToString());
                string path;
                if (user.PhotoPath == null)
                {
                    path = "C:\\Users\\user\\source\\repos\\BicardBackend\\BicardBackend\\Uploads\\PhotosOfUsers\\default.png";
                }
                else
                {
                    path = user.PhotoPath;
                }
                var tempObject = new 
                {
                    feedback.Id,
                    feedback.Message,
                    user.UserName,
                    UserPhoto = _fileService.ConvertFileToBase64(path).Result,
                    timeStamp = feedback.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                };
                tempList.Add(tempObject);
            }

            return Ok(tempList);
        }
        [HttpDelete("DeleteById/{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return Ok("Done.");
        }
    }
}
