namespace ExpenseTracker.Api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }      
        public required string Email { get; set; }
        public required string Password { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
