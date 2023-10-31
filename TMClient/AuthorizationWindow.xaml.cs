using System.Windows;
using TMServerLinker;
using TMServerLinker.Results;
namespace TMClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        TMServerLinker.TMClient client = null!;
        public AuthorizationWindow()
        {
            InitializeComponent();
        }
        public AuthorizationWindow(TMServerLinker.TMClient client)
        {
            this.client = client;
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                EmptyLoginLabel.Visibility = Visibility.Visible;
            }
            if (string.IsNullOrEmpty(PasswordTextBox.Text))
            {
                EmptyPasswordLabel.Visibility = Visibility.Visible;
            }
            if (string.IsNullOrEmpty(PasswordTextBox.Text) || string.IsNullOrEmpty(UsernameTextBox.Text)) return;
            var result = await client.LoginToAccount(UsernameTextBox.Text, PasswordTextBox.Text);
            if(result == LoginResult.ServerNotAviliable)
            {
                LoginStatusLabel.Content = "Server is not available";
                LoginStatusLabel.Visibility = Visibility.Visible;
            }
            else if(result == LoginResult.WrongLogin)
            {
                LoginStatusLabel.Content = "Username not found";
                LoginStatusLabel.Visibility = Visibility.Visible;
            }
            else if( result == LoginResult.WrongPassword)
            {
                LoginStatusLabel.Content = "Wrong password";
                LoginStatusLabel.Visibility = Visibility.Visible;
            }

        }

        private void UsernameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            EmptyLoginLabel.Visibility = Visibility.Hidden;
        }

        private void PasswordTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            EmptyPasswordLabel.Visibility = Visibility.Hidden;
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow(client);
            registrationWindow.Show();
            this.Close();
        }
    }
}
