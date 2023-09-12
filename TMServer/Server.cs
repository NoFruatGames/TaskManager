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
        public Server(string host, int port)
        {
            connectionHandler = new ConnectionHandler(new IPEndPoint(IPAddress.Parse(host), port));
            connectionHandler.ClientConnected += ConnectionHandler_ClientConnected; ;
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
            Console.WriteLine("Client connected");
            Stream stream = obj.GetStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("response from server");
            writer.Flush();
        }
        public void Dispose()
        {
            connectionHandler.Dispose();
        }
    }
    
}
