namespace ExpenseTracker.Api.Dto
{
    public record UserDto
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
