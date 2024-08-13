using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Trello.Selenium.Utils
{
    public static class WebDriverUtils
    {
        private static IWebDriver? _driver = new ChromeDriver(GetOptions());
        public static IWebDriver Driver
        {
            get
            {
                if (_driver == null)
                {
                    _driver = new ChromeDriver(GetOptions());
                }

                return _driver;
            }
        }

        public static IWebDriver GetWebDriver() => Driver; 
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

        public static void Quit()
        {
            _driver?.Quit();
            _driver = null;
        }
    }
}
