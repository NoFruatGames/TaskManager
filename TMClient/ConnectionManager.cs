using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using TMServerLinker.ConnectionHandlers;

namespace TMClient
{
    public class ConnectionManager : IDisposable
    {
        private string sessionToken = string.Empty;
        internal TMServerLinker.TMClient Client { get; private set; }
        public ConnectionManager(ConnectionHandler handler)
        {
            Client = new TMServerLinker.TMClient(handler);
        }
        internal async Task CloseSession(string sessionToken)
        {
            if (!string.IsNullOrEmpty(sessionToken))
            {
                await Client.ShutdownSession(sessionToken);
                
            }
            Client.Dispose();
            Application.Current.Shutdown();
        }
        public void Dispose()
        {
            Client?.Dispose();
            Application.Current.Shutdown();
        }


        private void DefaultWindowClosingHandler(object? sender, CancelEventArgs e)
        {
            Dispose();
        }

        internal void RegisterDefaultClosing(Window window)
        {
            window.Closing += DefaultWindowClosingHandler;
        }

        internal void UnregisterDefaultClosing(Window window)
        {
            window.Closing -= DefaultWindowClosingHandler;
        }


        private async void WindowClosingWithClosingHandler(object? sender, CancelEventArgs e)
        {
            await CloseSession(sessionToken);
        }

        internal void RegisterWithClosingSession(Window window, string sessionToken)
        {
            this.sessionToken = sessionToken;
            window.Closing += WindowClosingWithClosingHandler;
        }

        internal void UnRegisterWithClosingSession(Window window)
        {
            window.Closing -= WindowClosingWithClosingHandler;
        }
    }
}
