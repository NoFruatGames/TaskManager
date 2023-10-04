using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TransferDataTypes;
using TMServer.ConnectionTools.ConnectionProcessors;
using System.Reflection;

namespace TMServer.RequestProcessing
{
    internal class Clients : IDisposable
    {
        private List<ClientHandler> handlers = new List<ClientHandler>();

        public delegate Task MessageWithSender(Message message, ClientHandler sender);
        public event MessageWithSender? OnRequestRecived;
        public event Action<string>? OnClientDisconnected;


        public string InitHandler(IConnectionProcessor connection)
        {
            ClientHandler handler = new ClientHandler(connection);
            handler.ClientDisconnected += Client_ClientDisconnected;
            handler.OnMessageRecived += Client_OnMessageRecived;
            Thread reciveThread = new Thread(() => handler.HandleClientAsync());
            reciveThread.Start();
            handlers.Add(handler);
            return handler.Id;
        }
        private async Task Client_OnMessageRecived(Message message, ClientHandler connectionHandler)
        {
            if (OnRequestRecived is not null) await OnRequestRecived.Invoke(message, connectionHandler);
        }
        private void Client_ClientDisconnected(ClientHandler handler)
        {
            handler.Dispose();
            handlers.Remove(handler);
            OnClientDisconnected?.Invoke(handler.Id);
        }



        public ClientHandler? GetHandlerById(string id)
        {
            for (int i = 0; i < handlers.Count; ++i)
            {
                if (handlers[i].Id == id)
                    return handlers[i];
            }
            return null;
        }
        public List<ClientHandler> GetAllHandlers()
        {
            return handlers;
        }
        public void Dispose()
        {
            foreach (var item in handlers)
            {
                item.Dispose();
            }
        }
    }
}
