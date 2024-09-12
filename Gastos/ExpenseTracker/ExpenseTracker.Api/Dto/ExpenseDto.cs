namespace ExpenseTracker.Api.Dto
{
    public record ExpenseDto : BaseExpenseDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}