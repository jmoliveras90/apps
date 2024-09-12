namespace ExpenseTracker.Api.Dto
{
    public record LoginDto
    {
        public required string User { get; set; }
        public required string Password { get; set; }
    }
}
