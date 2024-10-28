using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Text.Json;
using Trello.Selenium.Dto;
using Trello.Selenium.Models;
using Trello.Selenium.UI;
using Trello.Selenium.Utils;

namespace Trello.Selenium
{
    public class SeleniunManager(string url, string login, string password,
        IEnumerable<string> columns, IEnumerable<string> tags, bool excluding, bool json, int timeout)
    {
        public static IWebDriver Driver => WebDriverUtils.GetWebDriver();
        public WebDriverWait Wait => WebDriverUtils.GetWebDriverWait(timeout);

        public IBoard Start()
        {
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl(url);

            Login();
            Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("trello-root")));

            FilterByTags(excluding);

            Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("trello-root")));

            return GetBoard(columns);
        }

        public void Login()
        {
            var loginLink = Wait.Until(ExpectedConditions.ElementIsVisible(By
                .XPath("/html/body/div[1]/div[2]/div[1]/div/main/div/div/div[2]/div[1]/div[2]/button")));

            loginLink.Click();

            var loginInput = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("username")));

            loginInput.SendKeys(login);

            var continueBtn = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("login-submit")));

            continueBtn.Click();

            var passwordInput = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("password")));

            passwordInput.SendKeys(password);

            var loginBtn = Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("login-submit")));

            loginBtn.Click();
        }

        public void FilterByTags(bool excluding)
        {
            Driver.Navigate()
               .GoToUrl($"{url}?filter={string.Join(",", tags.Select(tag => $"label:{tag}"))}{(excluding ? ",mode:and" : string.Empty)}");
        }

        public static IBoard GetBoard(IEnumerable<string> columns)
        {
            return new UI.Board(By.Id("board"), columns);
        }

        public void GetComments(BoardDto board)
        {
            if (json)
            {
                string json = string.Empty;

                do
                {
                    Thread.Sleep(50);
                    json = Wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("pre"))).Text;
                }
                while (!json.EndsWith("}"));

                var jsonData = JsonSerializer.Deserialize<Rootobject>(json);

                Driver.Quit();

                foreach (var column in board.Columns)
                {
                    foreach (var card in column.Cards)
                    {
                        card.Comment = jsonData?.actions
                            .Where(a => a.type == "commentCard" && a.data.card.name == card.Description)
                            .Select(x => x.data.text).FirstOrDefault() ?? string.Empty;
                    }
                }
            } 
            else
            {
                foreach (var column in board.Columns)
                {
                    foreach (var card in column.Cards)
                    {
                        Driver.Navigate().GoToUrl(card.Href);

                        Wait.Until(ExpectedConditions.ElementIsVisible(By
                                                   .ClassName("card-detail-window")));

                        Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("js-loading-card-actions")));
                        Wait.Until(ExpectedConditions.ElementExists(By.ClassName("js-list-actions")));

                        var comments = new List<string>();

                        try
                        {
                            comments = Driver.FindElement(By
                                                       .ClassName("current-comment"))
                            .FindElements(By.TagName("p")).Select(x => x.Text).ToList();
                        }
                        catch
                        {
                            comments = [];
                        }

                        card.Comment = string.Join(" ", comments);
                    }
                }
            }            
        }

        public void GoToJsonUrl()
        {
            Driver.Navigate().GoToUrl($"{url}.json");
        }
    }
}
