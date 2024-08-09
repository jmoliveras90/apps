using OpenQA.Selenium;
using Trello.Selenium.UI.Interfaces;

namespace Trello.Selenium.UI
{
    public class Card(IWebElement card) : BaseElement(card), ICard
    {
        private string _description { get; set; } = card.Text;
        private string _href { get; set; } = card.GetAttribute("href");

        public string Description => _description;

        public string Href => _href;

        public IEnumerable<string> Comments { get; set; } = new List<string>();
    }
}
