using ExpenseTracker.Api.Dto;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Services.Interfaces
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        Task<User> RegisterAsync(string username, string email, string password);
        Task<User?> AuthenticateAsync(string email, string password);
    }
}
