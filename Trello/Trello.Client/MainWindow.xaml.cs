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
        public IEnumerable<TagItem> Tags { get; set; } = new List<TagItem>();

        public MainWindow()
        {
            InitializeComponent();
            LoadConfiguration();
            SetValues();
        }

        private void OnBeginClick(object sender, RoutedEventArgs e)
        {
            var username = UserTextBox.Text;
            var password = PasswordBox.Password;
            var filter = FilterBox.Text ?? string.Empty;
            var excluding = Excluding.IsChecked ?? false;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Se debe introducir usuario y contraseña.", "Datos incompletos");
                return;
            }

            var selectedTags = TagListBox.SelectedItems.Cast<TagItem>().Select(t => t.Name);

            if (!selectedTags.Any()) {
                selectedTags = _configuration!.Tags;
            }

            try
            {
                SeleniumService.StartSelenium(_configuration!.Url, username, password, filter, selectedTags, excluding,
                    _configuration.Names, _configuration.Timeout, _configuration.Parallel);
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
        }

        private void SetValues()
        {
            DataContext = this;

            Tags = _configuration!.Tags.OrderBy(x => x)
                .Select(x => new TagItem
                {
                    Name = x
                });

            if (_configuration != null)
            {
                UserTextBox.Text = _configuration.Username;
                PasswordBox.Password = _configuration.Password;
            }
        }

        private void TagListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Excluding.IsEnabled = TagListBox.SelectedItems.Count > 0;
        }
    }

    public class TagItem
    {
        public required string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}