using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Trello.Selenium.Dto;
using Trello.Selenium.UI;
using Trello.Selenium.Utils;

namespace Trello.Selenium
{
    public class SeleniunManager(string url, string login, string password, int timeout)
    {
        public static IWebDriver Driver => WebDriverUtils.GetWebDriver();
        public WebDriverWait Wait => WebDriverUtils.GetWebDriverWait(timeout);

        public IBoard Start()
        {
            Driver.Manage().Window.Maximize();
            Driver.Navigate()
                .GoToUrl(url);

            Login();

            Wait.Until(ExpectedConditions.ElementIsVisible(By.Id("trello-root")));

            FilterByTag();
            return GetBoard();
        }

        public void Login()
        {
            var loginLink = Wait.Until(ExpectedConditions.ElementIsVisible(By
                .XPath("//*[@id=\"surface\"]/div/div/header[1]/div/div[1]/div[2]/a[1]")));

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

        public void FilterByTag()
        {
            Driver.Navigate()
               .GoToUrl($"{url}?filter=label:PORTUGAL");
        }

        public static IBoard GetBoard()
        {
            return new Board(By.Id("board"));
        }

        public void GetComments(BoardDto board, bool parallel)
        {
            if (parallel)
            {              
                LoadParallel(board);
            }
            else
            {
                LoadSequential(board);
            }           
        }


        private void LoadSequential(BoardDto board)
        {
            foreach (var column in board.Columns)
            {
                foreach (var card in column.Cards)
                {
                    Driver.Navigate().GoToUrl(card.Href);

                    Wait.Until(ExpectedConditions.ElementIsVisible(By
                                               .ClassName("card-detail-window")));

                    var comments = new List<string>();

                    try
                    {
                        comments = Wait.Until(x => Driver.FindElements(By
                                                   .ClassName("current-comment"))
                        .Select(c => c.FindElement(By.TagName("p")).Text)).ToList();
                    }
                    catch
                    {
                        comments = [];
                    }

                    card.Comments = comments;
                }
            }
        }

        private void LoadParallel(BoardDto board)
        {
            var cards = board.Columns.SelectMany(c => c.Cards).ToList();

            if (cards.Count == 0)
            {
                return;
            }

            foreach (var card in cards)
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("window.open('" + card.Href + "', '_blank');");
                Thread.Sleep(100);
            }

            for (int i = 1; i < Driver.WindowHandles.Count; i++)
            {
                string? handle = Driver.WindowHandles[i];
                Driver.SwitchTo().Window(handle);

                Wait.Until(ExpectedConditions.ElementIsVisible(By
                                               .ClassName("card-detail-window")));

                var title = Wait.Until(ExpectedConditions.ElementIsVisible(By
                                                                  .XPath("//*[@id=\"js-dialog-title\"]"))).Text;

                var comments = new List<string>();

                try
                {
                    comments = Wait.Until(x => Driver.FindElements(By
                                               .ClassName("current-comment"))
                    .Select(c => c.FindElement(By.TagName("p")).Text)).ToList();
                }
                catch
                {
                    comments = [];
                }

                cards.Single(c => c.Description == title).Comments = comments;
            }

            foreach (var column in board.Columns)
            {
                foreach (var card in column.Cards)
                {
                    card.Comments = cards.Single(c => c.ColumnIndex == card.ColumnIndex && c.Index == card.Index).Comments;
                }
            }
        }

        public static void Close()
        {
            Driver.Quit();
        }
    }
}
