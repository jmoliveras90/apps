using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;

namespace Report
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string selectedFilePath = string.Empty;
        private string defaultPassword = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            LoadAppSettings();
            ProcessButton.IsEnabled = false; // Deshabilitar el botón de procesar inicialmente
        }

        private void LoadAppSettings()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("configuration.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            defaultPassword = config["DefaultPassword"];
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

            string? password = string.IsNullOrEmpty(defaultPassword) ? PromptForPassword() : defaultPassword;

            // Verificar si el archivo está protegido con contraseña
            if (IsFileProtected())
            {
                // Solicitar la contraseña del archivo Excel

                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Se necesita una contraseña para abrir el archivo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Procesar el archivo con la contraseña
                ProcessExcelFile(password);
            }
            else
            {
                // Procesar el archivo sin contraseña
                ProcessExcelFile(null);
            }
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

        private void ProcessExcelFile(string? password)
        {
            try
            {
                if (password != null)
                {
                    // Usar EPPlus para abrir el archivo protegido con contraseña
                    using var package = new ExcelPackage(new FileInfo(selectedFilePath), password);
                    Process(package);
                }
                else
                {
                    // Usar EPPlus para abrir el archivo sin contraseña
                    using var package = new ExcelPackage(new FileInfo(selectedFilePath));
                    Process(package);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Process(ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets[0];

            // Mover los datos de la columna B a la columna AA
            int totalRows = worksheet.Dimension.End.Row;

            for (int i = 1; i <= totalRows; i++)
            {
                worksheet.Cells[i, 27].Value = worksheet.Cells[i, 2].Value; // Columna AA es la columna 27 (A=1, AA=27)

                worksheet.Cells[i, 2].Value = worksheet.Cells[i, 9].Value; // Columna B es la columna 2, columna I es la columna 9
                worksheet.Cells[i, 9].Value = null; // Limpiar la columna I después de mover los datos

                var valueG = worksheet.Cells[i, 7].Text; // Columna G (índice 7)
                var valueH = worksheet.Cells[i, 8].Text; // Columna H (índice 8)

                // Concatenar con " - " y colocar el resultado en la columna G
                worksheet.Cells[i, 7].Value = $"{valueG} - {valueH}";

                var cellValue = worksheet.Cells[i, 2].Text; // Obtener el texto de la columna B
                worksheet.Cells[i, 2].Value = cellValue.ToUpper(); // Convertir a mayúsculas

            }


            // Ordenar los datos de la columna B en orden ascendente, excluyendo la primera fila
            var dataRange = worksheet.Cells[2, 1, totalRows, worksheet.Dimension.End.Column]; // Rango desde la fila 2 hasta el final
            dataRange.Sort(x => x.SortBy.Column(1)); 

            // Eliminar las columnas H e I
            worksheet.DeleteColumn(8); // Eliminar columna H (índice 8)
            worksheet.DeleteColumn(8); // Eliminar columna I (que ahora es la nueva columna 8 después de eliminar la anterior)

            // Obtener la carpeta de Descargas del usuario
            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            // Guardar en un nuevo archivo con ticks de la fecha en la carpeta de Descargas
            string newFilePath = Path.Combine(downloadsFolder, $"DatosProcesados_{DateTime.Now.Ticks}.xlsx");
            package.SaveAs(new FileInfo(newFilePath));

            // Abrir el archivo automáticamente
            System.Diagnostics.Process.Start(new ProcessStartInfo(newFilePath) { UseShellExecute = true });

            MessageBox.Show($"Archivo procesado guardado en: {newFilePath}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private string? PromptForPassword()
        {
            // Usar un cuadro de diálogo personalizado para solicitar la contraseña
            PasswordWindow passwordWindow = new ();
            if (passwordWindow.ShowDialog() == true)
            {
                return passwordWindow.Password;
            }

            return null;
        }
    }
}