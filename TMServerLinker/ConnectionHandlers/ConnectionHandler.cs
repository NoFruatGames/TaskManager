using TransferDataTypes;

namespace TMServerLinker.ConnectionHandlers
{
    public abstract class ConnectionHandler :IDisposable
    {
        protected Queue<Message> Requests { get; set; } = new Queue<Message>();
        protected readonly string ServerHost;
        protected readonly int ServerPort;
        protected readonly object locker = new object();
        protected Thread reciveThread = null!;
        protected Thread sendThread = null!;

        internal abstract event Action<Message>? UpdateMessageRecived;
        internal abstract event Action<Message>? ResponseMessageRecived;
        internal abstract event Action? OnConnectionOpened;
        internal abstract event Action? OnConnectionClosed;
        internal abstract event Action<Message>? InitSessionMessageRecived;

        public bool IsConnected { get; protected set; }

        protected ConnectionHandler(string serverHost, int serverPort)
        {
            ServerHost = serverHost;
            ServerPort = serverPort;
        }
        internal abstract Task ConnectToServerAsync();
        internal void AddMessageToQueue(Message message)
        {
            Requests.Enqueue(message);
        }
        public abstract void Dispose();
    }
}
