using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TransferDataTypes;

namespace TMServer
{
    internal class Clients :IDisposable
    {
        private List<ClientHandler> clients = new List<ClientHandler>(); 
        public string RegisterClient(TcpClient obj)
        {
            ClientHandler client = new ClientHandler(obj);
            client.ClientDisconnected += Client_ClientDisconnected;
            client.OnRequestRecived += Client_OnRequestRecived;
            Thread reciveThread = new Thread(() => client.HandleClient());
            reciveThread.Start();
            clients.Add(client);
            return client.Id;
        }
        public delegate Task MessageWithSender(Message message, string senderId);
        public event MessageWithSender? OnRequestRecived;
        public event Action<string>? OnClientDisconnected;
        private async Task Client_OnRequestRecived(TransferDataTypes.Message message, ClientHandler sender)
        {
            if(OnRequestRecived is not null) await OnRequestRecived.Invoke(message, sender.Id);
        }

        public ClientHandler? GetClientById(string id)
        {
            for(int i = 0; i<clients.Count; ++i)
            {
                if (clients[i].Id == id)
                    return clients[i];
            }
            return null;
        }
        public List<ClientHandler> GetAllClients()
        {
            return clients;
        }
        public void Dispose()
        {
            foreach (var item in clients)
            {
                item.Dispose();
            }
        }
        private void Client_ClientDisconnected(ClientHandler obj)
        {
            try
            {
                obj.Dispose();
                clients.Remove(obj);
                OnClientDisconnected?.Invoke(obj.Id);
            }
            catch { }

        }
    }
}
