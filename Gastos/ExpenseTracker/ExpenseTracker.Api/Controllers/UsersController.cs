using AutoMapper;
using ExpenseTracker.Api.Dto;
using ExpenseTracker.Api.Infrastructure;
using ExpenseTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(ApplicationDbContext context, IMapper mapper) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<FullUserDto>> GetUser(Guid id)
        {
            var user = await context.Users
                .Include(u => u.Categories)
                .Include(u => u.Expenses)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = mapper.Map<FullUserDto>(user);

            return Ok(userDto);
        }
      
        [HttpPost]
        public async Task<ActionResult<UserDto>> RegisterUser(UserDto userDto)
        {
            if (await context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                return BadRequest("Email already in use");
            }

            var user = mapper.Map<User>(userDto);

            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, mapper.Map<UserDto>(user));
        }
       
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, UserDto userDto)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            mapper.Map(userDto, user);

            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            context.Entry(user).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return context.Users.Any(e => e.Id == id);
        }
    }
}
