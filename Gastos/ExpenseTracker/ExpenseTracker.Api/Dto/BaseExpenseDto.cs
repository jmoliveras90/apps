namespace ExpenseTracker.Api.Dto
{
    public record BaseExpenseDto
    {       
        public required string Description { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime? EndDate { get; set; }
    }
}