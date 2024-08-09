using Trello.Application.Excel;
using Trello.Selenium;
using Trello.Selenium.Dto;

namespace Trello.Application
{
    public class SeleniumService
    {
        public static void StartSelenium(string url, string user, string password)
        {
            var seleniumManager = new SeleniunManager(url, user, password);

            try
            {
                var board = seleniumManager.Start();

                var boardDto = new BoardDto
                {
                    Columns = board.Columns.Select(c => new ColumnDto
                    {
                        Title = c.Title,
                        Cards = c.Cards.Select(ca => new CardDto
                        {
                            Description = ca.Description,
                            Href = ca.Href,
                        }).ToList()
                    }).ToList()
                };

                SeleniunManager.GetComments(boardDto);
                ExportData(boardDto);
            }
            catch
            {
                throw;
            }
            finally
            {
                SeleniunManager.Close();
            }        
        }        

        public static void ExportData(BoardDto board)
        {
            ExcelExporter.ExportToExcel(board);
        }
    }
}
