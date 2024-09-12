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
    public class ExpensesController(ApplicationDbContext context, IMapper mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses()
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var entities = await context.Expenses.Include(x => x.Category)
                .Where(c => c.UserId == userId).ToListAsync();

            var result = entities.Select(x => mapper.Map<ExpenseDto>(x)).ToList();

            foreach (var expense in result)
            {
                expense.CategoryName = entities.Single(e => e.Id == expense.Id).Category.Name;
            }

            return result;
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> CreateExpense(BaseExpenseDto expense)
        {
            var userId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var entity = mapper.Map<Expense>(expense);

            entity.Id = new Guid();
            entity.UserId = userId;

            context.Expenses.Add(entity);
            await context.SaveChangesAsync();
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(Guid id, ExpenseDto expense)
        {
            if (id != expense.Id)
                return BadRequest();

            var entity = mapper.Map<Expense>(expense);

            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            var expense = await context.Expenses.FindAsync(id);
            if (expense == null)
                return NotFound();

            context.Expenses.Remove(expense);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
