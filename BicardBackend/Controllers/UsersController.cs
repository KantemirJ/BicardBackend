using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BicardBackend.Services;
using BicardBackend.DTOs;
using BicardBackend.Models;
using Microsoft.EntityFrameworkCore;
using BicardBackend.Data;

namespace BicardBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, IJwtService jwtService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Registration model)
        {
            var user = new User { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // You may customize the response based on your needs
                //await _userManager.AddToRoleAsync(user, "Patient");
                return Ok(new { Message = "User registered successfully" });
            }

            return BadRequest(new { result.Errors });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // You may customize the response based on your needs
                var user = await _userManager.FindByNameAsync(model.UserName);
                var accessToken = _jwtService.GenerateAccessToken(user);
                //Response.Cookies.Append("Bicard-Web-API-Access-Token", accessToken.Result, new CookieOptions()
                //{
                //    HttpOnly = true,
                //    SameSite = SameSiteMode.None,
                //    Secure = true,
                //    MaxAge = TimeSpan.FromSeconds(120)
                //});
                // Get the user's roles from the UserRoles table
                var roleIds = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                // Get the role names based on role IDs
                var roleNames = await _context.Roles
                    .Where(r => roleIds.Contains(r.Id))
                    .Select(r => r.Name)
                    .ToListAsync();

                // For simplicity, assuming the user has only one role (modify as needed)
                string roleName = roleNames.FirstOrDefault();
                return Ok(new
                {
                    userId = user.Id,
                    roleName = roleName,
                    userName = user.UserName,
                    accessToken = accessToken.Result,
                    Message = "Login successful"
                });
            }

            return Unauthorized(new { Message = "Invalid login attempt" });
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userManager.DeleteAsync(user);
            return Ok("Deleted successfully.");
        }
    }
}