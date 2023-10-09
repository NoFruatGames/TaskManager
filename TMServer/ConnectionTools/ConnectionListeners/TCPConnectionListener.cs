using System.Net;
using System.Net.Sockets;
using TMServer.ConnectionTools.Connections;
namespace TMServer.ConnectionTools.ConnectionListeners
{
    internal class TCPConnectionListener : IConnectionListener
    {
        public event Action<IConnection>? ConnectionOpened;
        public IPEndPoint EndPoint { get; init; }
        private TcpListener Listener { get; set; }
        private readonly object locker = new object();
        public TCPConnectionListener(IPEndPoint endPoint)
        {
            EndPoint = endPoint;

            Listener = new TcpListener(EndPoint);
        }
        public TCPConnectionListener(string listenerHost, int listenerPort)
        {
            EndPoint = new IPEndPoint(IPAddress.Parse(listenerHost), listenerPort);
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
        public void ListenConnections()
        {
            lock (locker)
            {
                try
                {
                    while (true)
                    {
                        TcpClient client = Listener.AcceptTcpClient();
                        TCPConnection connection = new TCPConnection(client);
                        ConnectionOpened?.Invoke(connection);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Connection handler error: {ex.Message}");
                }
                finally
                {
                    Dispose();
                }
            }

        }
        public void Dispose()
        {
            lock (locker)
            {
                Listener.Stop();
            }

        }
        public string GetListenerHost()
        {
            return EndPoint.Address.ToString();
        }
        public int GetListenerPort()
        {
            return EndPoint.Port;
        }
        public void Stop()
        {
            Dispose();
        }
    }
}
