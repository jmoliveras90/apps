using ClosedXML.Excel;
using System.Diagnostics;
using Trello.Selenium.Dto;

namespace Trello.Application.Excel
{
    public static class ExcelExporter
    {
        public static void ExportToExcel(BoardDto board, IEnumerable<string> names)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Portugal");
            var rowCount = 0;

            foreach (var column in board.Columns)
            {
                var currentRow = rowCount + 1;

                worksheet.Cell(currentRow, 2).Value = column.Title;
                var headerRange = worksheet.Range(currentRow, 2, currentRow, 4);

                headerRange.Merge();

                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightPink;
                headerRange.Style.Font.SetBold();

                foreach (var card in column.Cards)
                {
                    currentRow++;

                    var nameCell = worksheet.Cell(currentRow, 2);
                    var statusCell = worksheet.Cell(currentRow, 4);

                    nameCell.Style.Font.SetBold();
                    statusCell.Style.Font.SetBold();

                    nameCell.Value = names.FirstOrDefault(x => card.Description.Contains(x, StringComparison.CurrentCultureIgnoreCase)) ?? string.Empty;
                    worksheet.Cell(currentRow, 3).Value = card.Description;
                    statusCell.Value = card.Comment.Replace("\n", " ").Replace("\\", "");
                }

                rowCount += column.Cards.Count + 3;
            }

            worksheet.Columns().AdjustToContents();

            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string filePath = Path.Combine(downloadsFolder, $"Trello_{DateTime.Now.Ticks}.xlsx");
            workbook.SaveAs(filePath);

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}
