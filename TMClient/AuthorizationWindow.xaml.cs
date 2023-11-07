using System.Threading.Tasks;
using System;
using System.Windows;
using TMServerLinker.Results;
namespace TMClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        ConnectionManager connectionManager = null!;
        public AuthorizationWindow(ConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            this.connectionManager.RegisterDefaultClosing(this);
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
            var result = await connectionManager.Client.LoginToAccount(UsernameTextBox.Text, PasswordTextBox.Text);
            if (result.LoginStatus == LoginStatus.ServerNotAviliable)
            {
                LoginStatusLabel.Content = "Server is not available";
                LoginStatusLabel.Visibility = Visibility.Visible;
            }
            else if (result.LoginStatus == LoginStatus.WrongLogin)
            {
                LoginStatusLabel.Content = "Username not found";
                LoginStatusLabel.Visibility = Visibility.Visible;
            }
            else if (result.LoginStatus == LoginStatus.WrongPassword)
            {
                LoginStatusLabel.Content = "Wrong password";
                LoginStatusLabel.Visibility = Visibility.Visible;
            }
            else if (result.LoginStatus == LoginStatus.Success)
            {
                Global.SessionToken = result.SessionToken;
                Global.RememberSession = RememberCB.IsChecked ?? false;
                if (RememberCB.IsChecked == true)
                    Global.SaveSessionKeyToFile(Global.SessionToken);
                else
                {
                    connectionManager.UnregisterDefaultClosing(this);
                    connectionManager.RegisterWithClosingSession(this, Global.SessionToken);
                }
                connectionManager.UnregisterDefaultClosing(this);
                connectionManager.UnRegisterWithClosingSession(this);
                WorkWindow workWindow = new WorkWindow(connectionManager);
                workWindow.Show();
                this.Close();
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
            connectionManager.UnregisterDefaultClosing(this);
            connectionManager.UnRegisterWithClosingSession(this);
            RegistrationWindow registrationWindow = new RegistrationWindow(connectionManager);
            registrationWindow.Show();
            this.Close();
        }
    }
}
