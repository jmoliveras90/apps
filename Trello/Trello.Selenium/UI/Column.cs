using OpenQA.Selenium;
using Trello.Selenium.UI.Interfaces;

namespace Trello.Selenium.UI
{
    public class Column(IWebElement column) : BaseElement(column), IColumn
    {
        public string Title => Element.Text.Split("\r\n")[0];
        public ICollection<ICard> Cards => Element.FindElement(By.TagName("ol"))
            .FindElements(By.ClassName("KWQlnMvysRK4fI")).Select(c => new Card(c.FindElement(By.TagName("a"))))
            .Where(c => !string.IsNullOrEmpty(c.Element.Text)).ToArray();
    }
}
