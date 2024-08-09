using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using Trello.Selenium.Utils;

namespace Trello.Selenium.UI
{
    public class Board(By selector) : BaseElement(WebDriverUtils.GetWebDriverWait().Until(ExpectedConditions.ElementIsVisible(selector))), IBoard
    {
        public ICollection<Column> Columns => Element.FindElements(By.ClassName("tBRLg6uDC7sSyw"))
                .Select(c => new Column(c.FindElement(By.TagName("div"))))
                .Where(c => c.Title == "EN PREPARACIÓN" || c.Title == "UPBC" || c.Title == "PENDIENTE FIRMA").ToList();
    }
}
