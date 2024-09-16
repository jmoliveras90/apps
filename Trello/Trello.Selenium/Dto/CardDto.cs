namespace Trello.Selenium.Dto
{
    public class CardDto
    {
        public int Index { get; set; }
        public int ColumnIndex { get; set; }
        public required string Description { get; set; }
        public required string Href { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
