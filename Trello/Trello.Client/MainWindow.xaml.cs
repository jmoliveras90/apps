using System.Windows;
using System.Windows.Controls;
using Trello.Application;
using Trello.Client;

namespace Trello
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Configuration? _configuration { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            LoadConfiguration();
        }

        private void OnBeginClick(object sender, RoutedEventArgs e)
        {           
            var username = UserTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Se debe introducir usuario y contraseña.", "Datos incompletos");
                return;
            }

            try
            {
                SeleniumService.StartSelenium(_configuration!.Url, username, password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void LoadConfiguration()
        {
            string filePath = "./configuration.json";
            _configuration = JsonHelper.ReadConfiguration(filePath);

            if (_configuration != null)
            {
                UserTextBox.Text = _configuration.Username;
                PasswordBox.Password = _configuration.Password;
            }           
        }
    }
}