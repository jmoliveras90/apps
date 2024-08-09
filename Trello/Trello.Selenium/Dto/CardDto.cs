namespace Trello.Selenium.Dto
{
    public class CardDto
    {
        public required string Description { get; set; }
        public required string Href { get; set; }
        public IEnumerable<string> Comments { get; set; } = new List<string>();
    }
}
