using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Trello.Selenium.UI.Interfaces;
using Trello.Selenium.Utils;

namespace Trello.Selenium.UI
{
    public abstract class BaseElement(IWebElement element) : IElement
    {
        public IWebElement Element { get; protected set; } = element;

        public void Click()
        {
            WebDriverUtils.GetWebDriverWait().Until(x => Element.Displayed);
            Element.Click();
        }

        public void DoubleClick()
        {
            new Actions(WebDriverUtils.GetWebDriver()).DoubleClick(Element);
        }

        public void Type(string text)
        {
            Element.SendKeys(text);
        }

        public bool IsDisabled => Element.GetAttribute("disabled") == "true";
    }
}
