using System.Windows;

namespace Report
{
    public partial class PasswordWindow : Window
    {
        public string Password { get; private set; } = string.Empty;

        public PasswordWindow()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            Password = PasswordBox.Password;
            DialogResult = true;
            Close();
        }
    }
}