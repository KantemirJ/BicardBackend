using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BicardBackend.Services;
using BicardBackend.DTOs;
using BicardBackend.Models;
using Microsoft.EntityFrameworkCore;
using BicardBackend.Data;
using System.Reflection;
using static System.Net.WebRequestMethods;

namespace BicardBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager, IJwtService jwtService, ApplicationDbContext context, IFileService fileService, IEmailService emailService)
        {
            _fileService = fileService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Registration model)
        {
            var user = new User { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = $"https://localhost:7120/api/Users/ConfirmEmail/&userId={user.Id}?token={token}";
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ConfirmEmail.html");
                var template = await ReadTemplateFileAsync(templatePath);
                template = template.Replace("[CONFIRMATION_LINK]", link);
                template = template.Replace("[NUMBER]", "1");
                
                _emailService.Send(model.Email, "Подтвердите Ваш электронный адрес", template, "BicardSystem");
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
        [HttpGet("GetUsersByRole")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            // Check if the "Admin" role exists, and create it if not
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest("Role '" + role + "' does not exist.");
            }

            // Retrieve users with the "Admin" role and select only UserName and Id
            var list = await _userManager.GetUsersInRoleAsync(role);

            // Project the results to include only UserName and Id
            var DtoList = list.Select(user => new
            {
                Id = user.Id,
                UserName = user.UserName
            }).ToList();
            return Ok(DtoList);
        }
        [HttpPost("AddPhoto")]
        public async Task<IActionResult> AddPhoto(int userId, IFormFile photo)
        {
            if (photo == null)
            {
                return BadRequest();
            }
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return BadRequest();
            }
            user.PhotoPath = await _fileService.SaveFileAsync(photo, "PhotosOfUsers");
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        [HttpPost("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Missing user ID or confirmation token");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully");
            }

            return BadRequest(string.Join(",", result.Errors.Select(e => e.Description))); // Return validation errors
        }


        [HttpPost("SendPasswordResetLink")]
        public async Task<IActionResult> SendPasswordResetLink(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Invalid email address.");
            }
            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var template = await ReadTemplateFileAsync("BicardBackend.EmailTemplates.ResetPassword.html");
                template = template.Replace("[TOKEN]", token);
                template = template.Replace("[HOURS]", "1");
                template = template.Replace("[NAME]", user.UserName);
                _emailService.Send(email, "Сброс пароля для веб приложения клиники Бикард", template);
                
                return Ok("Password reset link has been sent to your email.");
            }
            catch (Exception ex)
            {
                // Handle exception during token generation (log the error)
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email, string token, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                return BadRequest("Missing required fields.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Invalid email address.");
            }

            try
            {
                var resetResult = await _userManager.ResetPasswordAsync(user, token, password);
                if (!resetResult.Succeeded)
                {
                    return BadRequest(string.Join(",", resetResult.Errors.Select(e => e.Description)));
                }

                return Ok("Password reset successful.");
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions (e.g., token expiration)
                return BadRequest("An error occurred while processing your request.");
            }
        }
        private async Task<string> ReadTemplateFileAsync(string filePath)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Email template not found: {filePath}");
                }

                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}