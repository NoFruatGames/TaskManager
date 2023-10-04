using System;
using TMServer.ConnectionTools.ConnectionProcessors;
namespace TMServer.ConnectionTools.ConnectionHandlers
{
    internal interface IConnectionHandler : IDisposable
    {
        event Action<IConnectionProcessor>? ClientConnected;
        void StartListener();
        void ListenClients();
        string GetHost();
        int GetPort();
    }
}
