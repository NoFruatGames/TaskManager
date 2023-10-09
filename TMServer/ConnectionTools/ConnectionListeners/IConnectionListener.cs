using System;
using TMServer.ConnectionTools.Connections;

namespace TMServer.ConnectionTools.ConnectionListeners
{
    internal interface IConnectionListener : IDisposable
    {
        event Action<IConnection>? ConnectionOpened;
        void StartListener();
        void ListenConnections();
        string GetListenerHost();
        int GetListenerPort();
        void Stop();
    }
}
