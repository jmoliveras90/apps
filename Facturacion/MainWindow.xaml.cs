using OfficeOpenXml;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using Facturacion.Dto;
using OfficeOpenXml.Style;
using System.Data;
using OfficeOpenXml.Table.PivotTable;
using OfficeOpenXml.Table;
using DocumentFormat.OpenXml.Spreadsheet;
using TableStyles = OfficeOpenXml.Table.TableStyles;
using System;

namespace Report
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string selectedFilePath = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ProcessButton.IsEnabled = false; // Deshabilitar el botón de procesar inicialmente
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Seleccionar archivo de Excel"
            };

            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                selectedFilePath = openFileDialog.FileName;
                FilePathTextBlock.Text = selectedFilePath;
                ProcessButton.IsEnabled = true; // Habilitar el botón de procesar cuando se seleccione un archivo
            }
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Por favor, seleccione un archivo de Excel primero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Verificar si el archivo está protegido con contraseña

            ProcessExcelFile();
        }

        private bool IsFileProtected()
        {
            try
            {
                using var package = new ExcelPackage(new FileInfo(selectedFilePath));
                // Si intentamos abrir el archivo y es necesario, el método ThrowIfWorkbookIsProtected lanzará una excepción
                return package.Workbook.Protection.LockStructure;
            }
            catch (InvalidOperationException)
            {
                // El archivo está protegido con contraseña
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password"))
                {
                    return true;
                }

                MessageBox.Show($"Error al verificar la protección del archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void ProcessExcelFile()
        {
            try
            {
                using var package = new ExcelPackage(new FileInfo(selectedFilePath));

                Process(package);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Process(ExcelPackage package)
        {
            var data = ReadExcelData(package);

            SaveExcelData(package, data);
        }

        private IEnumerable<OfertaDto> ReadExcelData(ExcelPackage package)
        {
            List<OfertaDto> records = [];

            var worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                if (OTC.IsChecked.GetValueOrDefault())
                {
                    records.Add(new OfertaDto
                    {
                        Propietario = worksheet.Cells[$"K{row}"].Text,
                        Campaña = worksheet.Cells[$"AD{row}"].Text,  // Columna AD
                        IdOferta = worksheet.Cells[$"G{row}"].Text,  // Columna G
                        EstadoContrato = string.IsNullOrEmpty(worksheet.Cells[$"J{row}"].Text) ? "Sin contrato" : worksheet.Cells[$"J{row}"].Text,  // Columna J
                        Nombre = worksheet.Cells[$"P{row}"].Text,  // Columna P
                        ImporteOferta = decimal.Parse(worksheet.Cells[$"AA{row}"].Text),  // Columna AA
                        NumCentros = short.TryParse(worksheet.Cells[$"S{row}"].Text, out short num) ? num : (short)0,  // Columna S
                        SubActividad = worksheet.Cells[$"D{row}"].Text,  // Columna D
                        FechaModificacion = DateTime.Parse(worksheet.Cells[$"F{row}"].Value.ToString()!), // Columna F
                        Etapa = worksheet.Cells[$"E{row}"].Text  // Columna E                  
                    });
                }
                else
                {
                    records.Add(new OfertaDto
                    {
                        Propietario = worksheet.Cells[row, 4].Text,  // Columna D
                        Campaña = worksheet.Cells[row, 5].Text,  // Columna E
                        IdOferta = worksheet.Cells[row, 6].Text,  // Columna F
                        EstadoContrato = string.IsNullOrEmpty(worksheet.Cells[row, 10].Text) ? "Sin contrato" : worksheet.Cells[row, 10].Text,  // Columna J
                        Nombre = worksheet.Cells[row, 14].Text,  // Columna N
                        ImporteOferta = decimal.Parse(worksheet.Cells[row, 20].Text),  // Columna T
                        NumCentros = short.TryParse(worksheet.Cells[row, 22].Text, out short num) ? num : (short)0,  // Columna V
                        SubActividad = worksheet.Cells[row, 24].Text,  // Columna X
                        FechaModificacion = DateTime.Parse(worksheet.Cells[row, 15].Value.ToString()!), // Columna O
                        Etapa = worksheet.Cells[row, 7].Text  // Columna G                    
                    });
                }
            }

            return records.Where(x => x.ImporteOferta >= 0);
        }

        private void SaveExcelData(ExcelPackage package, IEnumerable<OfertaDto> records)
        {
            var worksheet = package.Workbook.Worksheets.Add("Dinámica");

            worksheet.Cells[5, 2].Value = "Nombre".ToString();
            worksheet.Cells[5, 3].Value = "Id Oferta";
            worksheet.Cells[5, 4].Value = "Subactividad";
            worksheet.Cells[5, 5].Value = "Estado (Contrato asociado) (Contrato)";
            worksheet.Cells[5, 6].Value = "Nº Ofertas";
            worksheet.Cells[5, 7].Value = "Nº PS";
            worksheet.Cells[5, 8].Value = "Suma de Importe Oferta";
            worksheet.Cells[5, 9].Value = "Solución";
            worksheet.Cells[5, 10].Value = "Campaña";
            worksheet.Cells[5, 11].Value = "Etapa";
            worksheet.Cells[5, 12].Value = "Fecha de Modificacion";
            worksheet.Cells[5, 13].Value = "Año";
            worksheet.Cells[5, 14].Value = "Mes";
            worksheet.Cells[5, 15].Value = "Semana";
            worksheet.Cells[5, 16].Value = "Semana del año";

            var header = worksheet.Cells[5, 2, 5, 16];

            for (int i = 2; i <= 15; i++)
            {                
                worksheet.Column(i).Width = 12;
            }

             header.AutoFitColumns();

            header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            header.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(252, 100, 219)); // Color rosa cabecera  
            header.Style.Font.Color.SetColor(System.Drawing.Color.White);
            header.Style.Font.Bold = true;

            int row = 6;

            foreach (var record in records)
            {
                worksheet.Cells[row, 2].Value = record.Nombre;
                worksheet.Cells[row, 3].Value = record.IdOferta;
                worksheet.Cells[row, 4].Value = record.SubActividad;
                worksheet.Cells[row, 5].Value = record.EstadoContrato;
                worksheet.Cells[row, 6].Value = record.NumOfertas;
                worksheet.Cells[row, 7].Value = record.NumCentros;
                worksheet.Cells[row, 8].Value = record.ImporteOferta;
                worksheet.Cells[row, 9].Value = record.Solution;
                worksheet.Cells[row, 10].Value = string.IsNullOrEmpty(record.Campaña) ? record.Nombre.ToLower().Contains("prospec") ? "Prospección" : record.Campaña : record.Campaña;
                worksheet.Cells[row, 11].Value = record.Etapa;
                worksheet.Cells[row, 12].Value = record.FechaModificacion;
                worksheet.Cells[row, 12].Style.Numberformat.Format = "dd/MM/yyyy";
                worksheet.Cells[row, 13].Value = record.Año;
                worksheet.Cells[row, 14].Value = record.Mes;
                worksheet.Cells[row, 15].Value = record.Semana;
                worksheet.Cells[row, 16].Value = record.SemanaAño;

                worksheet.Cells[row, 2, row, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;

                if (row % 2 == 0)
                {
                    worksheet.Cells[row, 2, row, 16].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(254, 180, 238)); // Color rosa celdas pares
                }
                else
                {
                    worksheet.Cells[row, 2, row, 16].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(255, 255, 255)); // Color rosa celdas impares
                }

                row++;
            }

            var tableRange = worksheet.Cells[5, 2, row - 1, 16];
            var table = worksheet.Tables.Add(tableRange, "DataTable");

            tableRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);

            table.ShowFilter = true;

            // Agregar una fórmula de autosuma justo debajo de la tabla
            var sumaOfertas = worksheet.Cells[tableRange.End.Row + 1, 6];
            sumaOfertas.Formula = "SUBTOTAL(109, DataTable[Nº Ofertas])";

            var sumaPs = worksheet.Cells[tableRange.End.Row + 1, 7];
            sumaPs.Formula = "SUBTOTAL(109, DataTable[Nº PS])";

            var sumaImporte = worksheet.Cells[tableRange.End.Row + 1, 8];
            sumaImporte.Formula = "SUBTOTAL(109, DataTable[Suma de Importe Oferta])";

            // Etiqueta para la fila de autosuma
            worksheet.Cells[tableRange.End.Row + 1, 3].Value = "Total general";
            worksheet.Cells[tableRange.End.Row + 1, 1, tableRange.End.Row + 1, 8].Style.Font.Bold = true;

            AddSummaryTab(package, records);

            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            // Guardar en un nuevo archivo con ticks de la fecha en la carpeta de Descargas
            string newFilePath = Path.Combine(downloadsFolder, $"Facturacion_{DateTime.Now.Ticks}.xlsx");
            package.SaveAs(new FileInfo(newFilePath));

            // Abrir el archivo automáticamente
            System.Diagnostics.Process.Start(new ProcessStartInfo(newFilePath) { UseShellExecute = true });

            MessageBox.Show($"Archivo procesado guardado en: {newFilePath}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void AddSummaryTab(ExcelPackage package, IEnumerable<OfertaDto> records)
        {           
            var worksheet = package.Workbook.Worksheets.Add("Resumen");


            worksheet.Cells[1, 1].Value = "Nombre";
            worksheet.Cells[1, 2].Value = "Id Oferta";
            worksheet.Cells[1, 3].Value = "Subactividad";
            worksheet.Cells[1, 4].Value = "Estado (Contrato asociado) (Contrato)";
            worksheet.Cells[1, 5].Value = "Nº Ofertas";
            worksheet.Cells[1, 6].Value = "Nº PS";
            worksheet.Cells[1, 7].Value = "Suma de Importe Oferta";
            worksheet.Cells[1, 8].Value = "Solución";
            worksheet.Cells[1, 9].Value = "Campaña";
            worksheet.Cells[1, 10].Value = "Etapa";
            worksheet.Cells[1, 11].Value = "Fecha de Modificacion";
            worksheet.Cells[1, 12].Value = "Año";
            worksheet.Cells[1, 13].Value = "Mes";
            worksheet.Cells[1, 14].Value = "Semana";
            worksheet.Cells[1, 15].Value = "Semana del año";

            var header = worksheet.Cells[1, 1, 1, 15];

            int row = 2;

            foreach (var record in records)
            {
                worksheet.Cells[row, 1].Value = record.Nombre;
                worksheet.Cells[row, 2].Value = record.IdOferta;
                worksheet.Cells[row, 3].Value = record.SubActividad;
                worksheet.Cells[row, 4].Value = record.EstadoContrato;
                worksheet.Cells[row, 5].Value = record.NumOfertas;
                worksheet.Cells[row, 6].Value = record.NumCentros;
                worksheet.Cells[row, 7].Value = record.ImporteOferta;
                worksheet.Cells[row, 8].Value = record.Solution;
                worksheet.Cells[row, 9].Value = string.IsNullOrEmpty(record.Campaña) ? record.Nombre.ToLower().Contains("prospec") ? "Prospección" : record.Campaña : record.Campaña;
                worksheet.Cells[row, 10].Value = record.Etapa;
                worksheet.Cells[row, 11].Value = record.FechaModificacion;
                worksheet.Cells[row, 11].Style.Numberformat.Format = "dd/MM/yyyy";
                worksheet.Cells[row, 12].Value = record.Año;
                worksheet.Cells[row, 13].Value = record.Mes;
                worksheet.Cells[row, 14].Value = record.Semana;
                worksheet.Cells[row, 15].Value = record.SemanaAño;

                row++;
            }

            // Hide data columns
            for (int col = 1; col <= 15; col++)
            {
                worksheet.Column(col).Hidden = true;
            }

            // Rango tabla de datos
            var dataRange = worksheet.Cells[1, 1, row - 1, 15];
            var pivotTable = worksheet.PivotTables.Add(worksheet.Cells["P5"], dataRange, "TablaResumen");

            // Definir FILTROS
            pivotTable.PageFields.Add(pivotTable.Fields["Campaña"]);
            pivotTable.PageFields.Add(pivotTable.Fields["Año"]);
            pivotTable.PageFields.Add(pivotTable.Fields["Mes"]);
            pivotTable.PageFields.Add(pivotTable.Fields["Semana"]);
            pivotTable.PageFields.Add(pivotTable.Fields["Semana del año"]);
            pivotTable.PageFields.Add(pivotTable.Fields["Etapa"]);

            // Definir COLUMNAS            
            //pivotTable.ColumnFields.Add(pivotTable.Fields["Estado (Contrato asociado) (Contrato)"]);

            // Definir FILAS 
            pivotTable.RowFields.Add(pivotTable.Fields["Solución"]);
            pivotTable.RowFields.Add(pivotTable.Fields["Estado (Contrato asociado) (Contrato)"]);

            // Definir VALORES 
            var npsField = pivotTable.DataFields.Add(pivotTable.Fields["Nº PS"]);
            npsField.Function = DataFieldFunctions.Sum; // Usar SUMA
            npsField.Name = "Suma de Nº PS";

            var importeField = pivotTable.DataFields.Add(pivotTable.Fields["Suma de Importe Oferta"]);
            importeField.Function = DataFieldFunctions.Sum; // Usar SUMA
            importeField.Name = "Suma de Suma de Importe Oferta";

            pivotTable.MultipleFieldFilters = true;
            pivotTable.RowGrandTotals = true;
            pivotTable.ShowDrill = true;
            pivotTable.DataOnRows = false;

            pivotTable.RowHeaderCaption = "Solución"; // Evita que salga "Etiquetas de fila"

            // Vista informe tabular
            (from pf in pivotTable.Fields
             select pf).ToList().ForEach(f =>
             {
                 f.Compact = false;
                 f.Outline = false;
             });           

            pivotTable.PivotTableStyle = PivotTableStyles.Medium13; // Estilo de tabla dinámica rosa

            worksheet.Row(8).Hidden = true; // Oculta la fila 6 (Data)
        }
    }
}