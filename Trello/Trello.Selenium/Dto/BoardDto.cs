namespace Trello.Selenium.Dto
{
    public class BoardDto
    {
        public ICollection<ColumnDto> Columns { get; set; } = new List<ColumnDto>();
    }
}
