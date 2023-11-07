using System.Windows;

namespace TMClient
{
    /// <summary>
    /// Логика взаимодействия для WorkWindow.xaml
    /// </summary>
    public partial class WorkWindow : Window
    {
        ConnectionManager connectionManager;
        public WorkWindow(ConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            if (!Global.RememberSession)
                this.connectionManager.RegisterWithClosingSession(this, Global.SessionToken);
            else
                this.connectionManager.RegisterDefaultClosing(this);
            InitializeComponent();

        }

        private async void LogoutButton_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            connectionManager.UnRegisterWithClosingSession(this);
            connectionManager.UnregisterDefaultClosing(this);
            await connectionManager.Client.ShutdownSession(Global.SessionToken);
            Global.SessionToken = string.Empty;
            Global.SaveSessionKeyToFile(string.Empty);
            AuthorizationWindow authorizationWindow = new AuthorizationWindow(connectionManager);
            authorizationWindow.Show();
            this.Close();
        }
    }
}
