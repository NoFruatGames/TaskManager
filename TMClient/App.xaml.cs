using System;
using System.IO;
using System.Windows;
namespace TMClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        TMServerLinker.TMClient client;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AuthorizationWindow mainWindow = new AuthorizationWindow(client);
            mainWindow.Show();
        }
        public App()
        {
            Exit += App_Exit;
            client = new TMServerLinker.TMClient
            (
                new TMServerLinker.ConnectionHandlers.TCPConnectionHandler("192.168.0.129", 4980),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NoFruatINC", "TaskManager", "Client")
            );
        }
        private void App_Exit(object sender, ExitEventArgs e)
        {
            if (client is not null) client.Dispose();
        }
    }
}
