namespace ExpenseTracker.Api.Dto
{
    public record CategoryDto : BaseCategoryDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }        
    }
}
