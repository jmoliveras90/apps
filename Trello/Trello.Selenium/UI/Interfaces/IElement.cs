using OpenQA.Selenium;

namespace Trello.Selenium.UI.Interfaces
{
    public interface IElement
    {
        public IWebElement Element { get; }
        public void Click();
        public void DoubleClick();
        public void Type(string text);
    }
}
