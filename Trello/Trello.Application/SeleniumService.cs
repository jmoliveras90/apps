using Trello.Application.Excel;
using Trello.Selenium;
using Trello.Selenium.Dto;
using Trello.Selenium.Utils;

namespace Trello.Application
{
    public class SeleniumService
    {
        public static void StartSelenium(string url, string user, string password, IEnumerable<string> names, int timeout, bool parallel)
        {
            var seleniumManager = new SeleniunManager(url, user, password, timeout);

            try
            {
                var board = seleniumManager.Start();
                var boardDto = new BoardDto
                {
                    Columns = board.Columns.Select((column, i) => new ColumnDto
                    {
                        Title = column.Title,
                        Cards = column.Cards.Select((card, j) => new CardDto
                        {
                            Index = j,
                            ColumnIndex = i,
                            Description = card.Description,
                            Href = card.Href,
                        }).ToList()
                    }).ToList()
                };

                seleniumManager.GetComments(boardDto, parallel);
                ExportData(boardDto, names);
            }
            catch
            {
                throw;
            }
            finally
            {
                WebDriverUtils.Quit();
            }        
        }        

        public static void ExportData(BoardDto board, IEnumerable<string> names)
        {
            ExcelExporter.ExportToExcel(board, names);
        }
    }
}
