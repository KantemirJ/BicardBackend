using BicardBackend.Models;
using Microsoft.AspNetCore.Identity;

namespace BicardBackend.Services
{
    public interface IJwtService
    {
        Task<string> GenerateAccessToken(User user);
    }
}
