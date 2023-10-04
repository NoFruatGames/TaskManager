using System.Net;
using System.Net.Sockets;
using TMServer.ConnectionTools.ConnectionProcessors;
namespace TMServer.ConnectionTools.ConnectionHandlers
{
    internal class TCPConnectionHandler : IConnectionHandler
    {
        public event Action<IConnectionProcessor>? ClientConnected;
        public IPEndPoint EndPoint { get; init; }
        private TcpListener Listener { get; set; }
        private readonly object locker = new object();
        public TCPConnectionHandler(IPEndPoint endPoint)
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
            lock (locker)
            {
                try
                {
                    while (true)
                    {
                        TcpClient client = Listener.AcceptTcpClient();
                        TCPConnectionProcessor connection = new TCPConnectionProcessor(client);
                        ClientConnected?.Invoke(connection);
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
            lock (locker)
            {
                Listener.Stop();
            }

        }
        public string GetHost()
        {
            return EndPoint.Address.ToString();
        }
        public int GetPort()
        {
            return EndPoint.Port;
        }
    }
}
