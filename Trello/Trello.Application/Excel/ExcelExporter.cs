﻿using ClosedXML.Excel;
using System.Diagnostics;
using Trello.Selenium.Dto;

namespace Trello.Application.Excel
{
    public static class ExcelExporter
    {
        public static void ExportToExcel(BoardDto board)
        {
            using (var workbook = new XLWorkbook())
            {
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

                        worksheet.Cell(currentRow, 2).Value = "Nombre";
                        worksheet.Cell(currentRow, 3).Value = card.Description;
                        worksheet.Cell(currentRow, 4).Value = card.Comments.FirstOrDefault() ?? string.Empty;
                    }

                    rowCount += column.Cards.Count + 3;
                }

                worksheet.Columns().AdjustToContents();

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"Portugal_{DateTime.Now.Ticks}.xlsx");
                workbook.SaveAs(filePath);

                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
        }
    }
}