namespace ExpenseTracker.Api.Models
{
    public class Expense
    {
        public Guid Id { get; set; } 
        public required string Description { get; set; }
        public required decimal Amount { get; set; }
        public required DateTime Date { get; set; }
        public required Guid CategoryId { get; set; }
        public bool IsRecurring { get; set; } 
        public DateTime? EndDate { get; set; }
        public required Guid UserId { get; set; }
        public required Category Category { get; set; }        
        public required User User { get; set; }
    }
}
