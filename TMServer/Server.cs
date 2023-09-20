using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace TMServer
{
    public class Server :IDisposable
    {
        private ConnectionHandler connectionHandler;
        private List<ClientHandler> clients;
        private Profiles profiles;
        private RequestProcessor requestProcessor;
        public Server(string host, int port)
        {
            connectionHandler = new ConnectionHandler(new IPEndPoint(IPAddress.Parse(host), port));
            connectionHandler.ClientConnected += ConnectionHandler_ClientConnected; ;
            clients = new List<ClientHandler>();
            profiles = new Profiles();
            requestProcessor = new RequestProcessor() 
            {
                profiles= profiles 
            };
        }
        public void Start()
        {
            try
            {
                connectionHandler.StartListener();
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Server started at {connectionHandler.EndPoint.Address}:{connectionHandler.EndPoint.Port}");
                Thread listenClientsThread = new Thread(connectionHandler.ListenClients);
                listenClientsThread.Name = "connection listener thread";
                listenClientsThread.Start();
                while (true)
                {
                    Console.Write(">");
                    string? command = Console.ReadLine();
                    if(command == "show clients")
                    {
                        foreach (var c in clients)
                        {
                            Console.WriteLine(c);
                        }
                        Console.WriteLine("-------------------------------------------------------------------------------");
                    }
                    else if(command == "show profiles")
                    {
                        for(int i = 0; i < profiles.Count; ++i)
                        {
                            Console.WriteLine(profiles.GetById(i));
                        }
                        Console.WriteLine("-------------------------------------------------------------------------------");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Dispose();
            }
        }
        private void ConnectionHandler_ClientConnected(TcpClient obj)
        {
            ClientHandler client = new ClientHandler(obj);
            client.ClientDisconnected += Client_ClientDisconnected;
            client.OnRequestRecived += Client_OnRequestRecived;
            clients.Add(client);
            Thread reciveThread = new Thread(()=>client.HandleClient());
            reciveThread.Start();
        }

        private async Task Client_OnRequestRecived(TransferDataTypes.Message message, ClientHandler sender)
        {
            TransferDataTypes.Message response = requestProcessor.Process(message);
            await sender.SendMessageAsync(response.Serialize());
        }

        private void Client_ClientDisconnected(ClientHandler obj)
        {
            try { obj.Dispose(); } catch { }      
            clients.Remove(obj);
        }

        public void Dispose()
        {
            try
            {
                connectionHandler.Dispose();
                foreach (var c in clients)
                {
                    c.Dispose();
                }
            }
            catch { }

        }
    }
    
}
