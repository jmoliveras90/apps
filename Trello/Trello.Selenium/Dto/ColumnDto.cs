using OpenQA.Selenium;
using Trello.Selenium.UI.Interfaces;
using Trello.Selenium.UI;

namespace Trello.Selenium.Dto
{
    public class ColumnDto
    {
        public required string Title { get; set; }
        public ICollection<CardDto> Cards { get; set; } = new List<CardDto>();
    }
}
