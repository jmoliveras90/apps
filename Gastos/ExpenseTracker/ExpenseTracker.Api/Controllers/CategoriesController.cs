using AutoMapper;
using ExpenseTracker.Api.Dto;
using ExpenseTracker.Api.Infrastructure;
using ExpenseTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTracker.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ApplicationDbContext context, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            return await context.Categories.Where(c => c.UserId == userId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(BaseCategoryDto category)
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var entity = mapper.Map<Category>(category);

            entity.Id = new Guid();
            entity.UserId = userId;

            context.Categories.Add(entity);
            await context.SaveChangesAsync();
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryDto category)
        {
            if (id != category.Id)
                return BadRequest();

            var entity = mapper.Map<Category>(category);

            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await context.Categories.FindAsync(id);
            
            if (category == null)
                return NotFound();

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
