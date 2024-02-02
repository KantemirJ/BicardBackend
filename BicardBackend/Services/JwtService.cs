using BicardBackend.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace BicardBackend.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public JwtService(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task<string> GenerateAccessToken(User user)
        {
            // Get JWT configuration values from appsettings.json
            string key = _configuration["Jwt:Key"];
            string issuer = _configuration["Jwt:Issuer"];
            string audience = _configuration["Jwt:Audience"];

            // Retrieve additional claims from the IdentityUser
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                // Include additional claims as needed
            };

            // Retrieve the roles of the user
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create a key using the secret key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expiration time
                signingCredentials: credentials
            );

            // Serialize the token to a string
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }
}
