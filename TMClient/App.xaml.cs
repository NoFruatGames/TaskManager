using System.Windows;
using TMServerLinker.Results;
namespace TMClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        ConnectionManager connectionManager;

        public App()
        {
            connectionManager = new ConnectionManager(new TMServerLinker.ConnectionHandlers.TCPConnectionHandler("192.168.0.129", 4980));
        }
        protected override  async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Global.SessionToken = Global.GetSessionKeyFromFile();
            var result = await connectionManager.Client.InitSession(Global.SessionToken);
            if(result == InitSessionResult.Success)
            {
                Global.RememberSession = true;
                WorkWindow workWindow = new WorkWindow(connectionManager);
                workWindow.Show();
            }
            else if(result == InitSessionResult.TokenAlreadyUsing)
            {
                MessageBox.Show("another application instance is already running");
                connectionManager.Dispose();
                Application.Current.Shutdown();
            }
            else
            {
                Global.SessionToken = string.Empty;
                Global.SaveSessionKeyToFile(string.Empty);
                AuthorizationWindow mainWindow = new AuthorizationWindow(connectionManager);
                mainWindow.Show();
            }
            
        }


    }
}
