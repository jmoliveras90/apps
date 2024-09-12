namespace ExpenseTracker.Api.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required User User { get; set; }
        public required string Name { get; set; }
    }
}
