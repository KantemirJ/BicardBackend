using BicardBackend.Data;
using BicardBackend.DTOs;
using BicardBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public FeedbacksController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(FeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }
            Feedback feedback = new()
            {
                Message = feedbackDto.Message,
                UserId = feedbackDto.UserId,
                DoctorId = feedbackDto.DoctorId
            };
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
            
            return Ok(new
            {
                feedback.Id,
                feedback.Message,
                user.UserName,
                timeStamp = feedback.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        [HttpGet("GetFeedbacksByDoctorId/{id}")]
        public async Task<IActionResult> GetFeedbacksByDoctorId(int id)
        {
            var listOfFeedback = await _context.Feedbacks.Where(f => f.DoctorId == id).ToListAsync();
            if (listOfFeedback == null)
            {
                return NotFound();
            }
            var tempList = new List<object>();
            foreach (var feedback in listOfFeedback)
            {
                var user = await _userManager.FindByIdAsync(feedback.UserId.ToString());
                var tempObject = new
                {
                    feedback.Id,
                    feedback.Message,
                    user.UserName,
                    timeStamp = feedback.Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                };
                tempList.Add(tempObject);
            }

            return Ok(tempList);
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
                var tempObject = new 
                {
                    feedback.Id,
                    feedback.Message,
                    user.UserName,
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
