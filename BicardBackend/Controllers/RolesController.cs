using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BicardBackend.Models;
using BicardBackend.Data;
using BicardBackend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BicardBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public RolesController(RoleManager<Role> roleManager, UserManager<User> userManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null)
            {
                // Assuming roleName is the name of the role you want to assign
                var roleExists = await _roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    var role = new Role(roleName);
                    await _roleManager.CreateAsync(role);
                }
                bool hasAnyRole = await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id);
                if(hasAnyRole)
                {
                    return BadRequest("User already has a role.");
                }
                await _userManager.AddToRoleAsync(user, roleName);
            }
            return Ok();
        }
        [HttpPost("UnassignRole")]
        public async Task<IActionResult> UnassignRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null)
            {
                // Check if the user is already in the specified role
                var isInRole = await _userManager.IsInRoleAsync(user, roleName);

                if (isInRole)
                {
                    // Remove the user from the role
                    var result = await _userManager.RemoveFromRoleAsync(user, roleName);

                    if (result.Succeeded)
                    {
                        return Ok("User is unassigned from role");
                    }
                    else
                    {
                        return new ObjectResult(new { error = "Internal Server Error", message = "Can't unassign user from role" })
                        {
                            StatusCode = 500
                        };
                    }
                }
                else
                {
                    return BadRequest($"User is not in \"{roleName}\" role");
                }
            }
            else
            {
                return NotFound($"User with name {userName} not found");
            }
        }
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name cannot be empty");
            }
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (roleExists)
            {
                return BadRequest("Role already exists.");
            }
            var role = new Role(roleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok($"Role '{roleName}' created successfully");
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("GetRoles")]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles;
            return Ok(roles);
        }

        [HttpDelete("{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                return NotFound($"Role '{roleName}' not found");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return Ok($"Role '{roleName}' deleted successfully");
            }

            return BadRequest(result.Errors);
        }
    }

}
