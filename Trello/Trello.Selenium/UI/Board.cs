using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using Trello.Selenium.Utils;

namespace Trello.Selenium.UI
{
    public class Board(By selector, IEnumerable<string> columns) : BaseElement(WebDriverUtils.GetWebDriverWait().Until(ExpectedConditions.ElementIsVisible(selector))), IBoard
    {
        public ICollection<Column> Columns => Element.FindElements(By.ClassName("tBRLg6uDC7sSyw"))
                .Select(c => new Column(c.FindElement(By.TagName("div"))))
                .Where(c => columns.Contains(c.Title)).ToList();
    }
}
