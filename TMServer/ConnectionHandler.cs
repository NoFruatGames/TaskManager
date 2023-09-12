using System;
using System.Net.Sockets;
using System.Net;
namespace TM_Server
{
    internal class ConnectionHandler :IDisposable
    {
        public event Action<TcpClient>? ClientConnected;
        public IPEndPoint EndPoint { get; set; }
        private TcpListener Listener { get; set; }
        private readonly object locker = new object();
        public ConnectionHandler(IPEndPoint endPoint)
        {
            EndPoint = endPoint;

            Listener = new TcpListener(EndPoint);
        }
        public void StartListener()
        {
            try
            {
                Listener.Start();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void ListenClients()
        {
            lock(locker)
            {
                try
                {
                    while (true)
                    {
                        TcpClient client = Listener.AcceptTcpClient();
                        ClientConnected?.Invoke(client);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Connection handler error: {ex.Message}");
                }
                finally
                {
                    Listener.Stop();
                }
            }
            
        }

        public void Dispose()
        {
            Listener.Stop();
        }
    }
}
