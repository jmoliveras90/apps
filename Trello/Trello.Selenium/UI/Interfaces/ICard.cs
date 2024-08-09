using OpenQA.Selenium;

namespace Trello.Selenium.UI.Interfaces
{
    public interface ICard : IElement
    {
        public string Description { get; }
        public string Href { get; }
    }
}
