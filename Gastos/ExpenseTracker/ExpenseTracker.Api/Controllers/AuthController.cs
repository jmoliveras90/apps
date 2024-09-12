using ExpenseTracker.Api.Dto;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto dto)
        {
            var user = await _authService.RegisterAsync(dto.Username, dto.Email, dto.Password);
            if (user == null)
            {
                return BadRequest("Error during registration");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { user.Username, user.Email, Token = token });
        }

        // Inicio de sesión
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _authService.AuthenticateAsync(dto.User, dto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { user.Username, user.Email, Token = token });
        }
    }
}