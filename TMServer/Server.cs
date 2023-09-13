using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TM_Server;

namespace TMServer
{
    internal class Server :IDisposable
    {
        private ConnectionHandler connectionHandler;
        private List<ClientHandler> clients;
        public Server(string host, int port)
        {
            connectionHandler = new ConnectionHandler(new IPEndPoint(IPAddress.Parse(host), port));
            connectionHandler.ClientConnected += ConnectionHandler_ClientConnected; ;
            clients = new List<ClientHandler>();
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
            client.CreateAccountRequest += Client_CreateAccountRequest;
            clients.Add(client);
            Thread reciveThread = new Thread(client.HandleClient);
            reciveThread.Start();
        }

        private CreateAccountResult Client_CreateAccountRequest(TransferDataTypes.Payloads.PayloadAccountInfo payload)
        {
            Thread.Sleep(10000);
            return CreateAccountResult.Success;
        }

        private void Client_ClientDisconnected(ClientHandler obj)
        {
            clients.Remove(obj);
        }

        public void Dispose()
        {
            connectionHandler.Dispose();
            foreach(var c in clients)
            {
                c.Dispose();
            }
        }
    }
    
}
