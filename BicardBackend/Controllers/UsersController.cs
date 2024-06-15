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
using System.Data;
using System.Text;

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
                var roleExists = await _roleManager.RoleExistsAsync("Patient");
                if (!roleExists)
                {
                    var role = new Role("Patient");
                    await _roleManager.CreateAsync(role);
                }
                await _userManager.AddToRoleAsync(user, "Patient");
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string encodedToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(token))
                                                .Replace('+', '-')  
                                                .Replace('/', '_');
                var link = $"https://localhost:3000/confirm-email?userId={user.Id}&token={encodedToken}";
                var template = await _emailService.ReadTemplateFileAsync("BicardBackend.EmailTemplates.ConfirmEmail.html");
                template = template.Replace("[CONFIRMATION_LINK]", link);
                template = template.Replace("[NUMBER]", "1");
                template = template.Replace("[NAME]", user.UserName);
                //Random rnd = new Random();
                //int number = rnd.Next(100000, 1000000);
                //var code = number.ToString("D6");
                //var template = await _emailService.ReadTemplateFileAsync("BicardBackend.EmailTemplates.ConfirmEmail.html");
                //template = template.Replace("[CODE]", code);
                //template = template.Replace("[NUMBER]", "1");
                //template = template.Replace("[NAME]", user.UserName);

                //_emailService.Send(model.Email, "Подтвердите Ваш электронный адрес", template);
                return Ok();
            }

            return BadRequest(new { result.Errors });
        }
        [HttpGet("ResendConfirmationEmail")]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string encodedToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(token))
                                               .Replace('+', '-')
                                               .Replace('/', '_');
                var link = $"https://localhost:3000/confirm-email?userId={user.Id}&token={encodedToken}";
                var template = await _emailService.ReadTemplateFileAsync("BicardBackend.EmailTemplates.ConfirmEmail.html");
                template = template.Replace("[CONFIRMATION_LINK]", link);
                template = template.Replace("[NUMBER]", "1");
                template = template.Replace("[NAME]", user.UserName);
                //Random rnd = new Random();
                //int number = rnd.Next(100000, 1000000);
                //var code = number.ToString("D6");
                //var template = await _emailService.ReadTemplateFileAsync("BicardBackend.EmailTemplates.ConfirmEmail.html");
                //template = template.Replace("[CODE]", code);
                //template = template.Replace("[NUMBER]", "1");
                //template = template.Replace("[NAME]", user.UserName);

                _emailService.Send(email, "Подтвердите Ваш электронный адрес", template);
                return Ok();
            }

            return BadRequest("Invalid email address");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid login attempt" });
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);

            if (result.Succeeded)
            {
                var accessToken = _jwtService.GenerateAccessToken(user);

                var roleName = await _context.UserRoles
                      .Where(ur => ur.UserId == user.Id)
                      .Join(
                          _context.Roles,
                          ur => ur.RoleId,
                          r => r.Id,
                          (ur, r) => r.NormalizedName
                      )
                      .FirstOrDefaultAsync();
                return Ok(new
                {
                    userId = user.Id,
                    roleName,
                    userName = user.UserName,
                    accessToken = accessToken.Result,
                    Message = "Login successful"
                });
            }

            return Unauthorized(new { Message = "Invalid login attempt" });
        }
        [HttpPost("LoginWeb")]
        public async Task<IActionResult> LoginWeb([FromBody] LoginWeb model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid login attempt" });
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);

            if (result.Succeeded)
            {
                var accessToken = _jwtService.GenerateAccessToken(user);

                var roleName = await _context.UserRoles
                      .Where(ur => ur.UserId == user.Id)
                      .Join(
                          _context.Roles,
                          ur => ur.RoleId,
                          r => r.Id,
                          (ur, r) => r.NormalizedName
                      )
                      .FirstOrDefaultAsync();
                return Ok(new
                {
                    userId = user.Id,
                    roleName,
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
        
        [HttpPost("UpdatePhoto")]
        public async Task<IActionResult> UpdatePhoto(int userId, IFormFile photo)
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
            if(user.PhotoPath != null)
            {
                _fileService.DeleteFile(user.PhotoPath);
            }
            user.PhotoPath = await _fileService.SaveFileAsync(photo, "PhotosOfUsers");
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("ConfirmEmail")]
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
            string decodedToken = Encoding.ASCII.GetString(
                Convert.FromBase64String(token.Replace('-', '+').Replace('_', '/'))
            );

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                return Ok("Email confirmation successful.");
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
                Random rnd = new Random();
                int number = rnd.Next(100000, 1000000);
                var code = number.ToString("D6");
                var template = await _emailService.ReadTemplateFileAsync("BicardBackend.EmailTemplates.ResetPassword.html");
                template = template.Replace("[CODE]", code);
                template = template.Replace("[HOURS]", "1");
                template = template.Replace("[NAME]", user.UserName);
                //_emailService.Send(email, "Сброс пароля для веб приложения клиники Бикард", template);

                return Ok(new { PasswordResetToken = token, code, user.Email, Message = "Мы отправили код на вашу почту." });
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
        [HttpGet("GetProfileIfno")]
        public async Task<IActionResult> GetProfileIfno(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                user.UserName,
                user.Email,
                user.PhoneNumber,
                Age = DateTime.Now.Year - user.BirthDay.Year,
                user.Sex,
                user.PhotoPath
            });
        }
        [HttpPost("UpdateProfileIfno")]
        public async Task<IActionResult> UpdateProfileIfno([FromQuery] int id,[FromForm] UserProfile dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            user.UserName = dto.UserName;
            user.NormalizedUserName = dto.UserName.ToUpper();
            user.Email = dto.Email;
            user.NormalizedEmail = dto.Email.ToUpper();
            user.PhoneNumber = dto.PhoneNumber;
            user.BirthDay = dto.BirthDay.ToUniversalTime();
            user.Sex = dto.Sex;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }
        
    }
}