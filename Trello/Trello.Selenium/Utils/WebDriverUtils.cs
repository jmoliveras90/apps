using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Trello.Selenium.Utils
{
    public static class WebDriverUtils
    {
        private static readonly IWebDriver _driver = new ChromeDriver(GetOptions());
        
        public static IWebDriver GetWebDriver() => _driver; 
        public static WebDriverWait GetWebDriverWait(int seconds = 30)
        {
            return new WebDriverWait(_driver, TimeSpan.FromSeconds(seconds));
        }

        public static ChromeOptions GetOptions()
        {
            var result = new ChromeOptions
            {
                PageLoadStrategy = PageLoadStrategy.None
            };

            result.AddArgument("--incognito");
            result.AddArgument("--disable-search-engine-choice-screen");

            return result;
        }
    }
}
