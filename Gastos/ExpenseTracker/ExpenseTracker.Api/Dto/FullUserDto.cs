namespace ExpenseTracker.Api.Dto
{
    public record FullUserDto : UserDto
    {
        public List<CategoryDto> Categories { get; set; } = [];
        public List<ExpenseDto> Expenses { get; set; } = [];
    }
}
