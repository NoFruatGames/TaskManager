using System.Windows;
using TMServerLinker;
using TMServerLinker.Results;

namespace TMClient
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        TMServerLinker.TMClient client = null!;
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        public RegistrationWindow(TMServerLinker.TMClient client)
        {
            this.client = client;
            InitializeComponent();
        }

        private void EmailTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            EmptyEmailLabel.Visibility = Visibility.Hidden;
        }
        private void UsernameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            EmptyLoginLabel.Visibility = Visibility.Hidden;
        }
        private void PasswordTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            EmptyPasswordLabel.Visibility = Visibility.Hidden;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                EmptyLoginLabel.Visibility = Visibility.Visible;
            }
            if (string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                EmptyPasswordLabel.Visibility = Visibility.Visible;
            }
            if(string.IsNullOrEmpty(EmailTextBox.Text))
            {
                EmptyEmailLabel.Visibility = Visibility.Visible;
            }
            if (string.IsNullOrEmpty(PasswordTextBox.Text) || string.IsNullOrEmpty(UsernameTextBox.Text) || string.IsNullOrEmpty(EmailTextBox.Text)) return;
            var res = await client.RegisterAccount
            (
                new Profile()
                {
                    Username = UsernameTextBox.Text,
                    Password = PasswordTextBox.Text,
                    Email = EmailTextBox.Text
                }
            );
            if(res == RegisterResult.ServerNotAviliable)
            {
                RegisterStatusLabel.Visibility = Visibility.Visible;
                RegisterStatusLabel.Content = "Server is not aviliable";
            }
            else if (res == RegisterResult.UsernameExist)
            {
                RegisterStatusLabel.Visibility = Visibility.Visible;
                RegisterStatusLabel.Content = "Username already exist";
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs args )
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow(client);
            authorizationWindow.Show();
            this.Close();
        }
    }
}
