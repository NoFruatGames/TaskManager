using TransferDataTypes;

namespace TMServerLinker.ConnectionHandlers
{
    public abstract class ConnectionHandler :IDisposable
    {
        protected Queue<TMMessage> Requests { get; set; } = new Queue<TMMessage>();
        protected readonly string ServerHost;
        protected readonly int ServerPort;
        protected readonly object locker = new object();
        protected Thread reciveThread = null!;
        protected Thread sendThread = null!;

        internal abstract event Action<TMMessage>? MessageRecived;
        internal abstract event Action? OnConnectionOpened;
        internal abstract event Action? OnConnectionClosed;
        internal bool NeedToReconnect { get; set; } = true;

        public bool IsConnected { get; protected set; }

        protected ConnectionHandler(string serverHost, int serverPort)
        {
            ServerHost = serverHost;
            ServerPort = serverPort;
        }
        internal abstract Task ConnectToServerAsync();
        internal void AddMessageToQueue(TMMessage message)
        {
            Requests.Enqueue(message);
        }
        public abstract void Dispose();

    }
}
