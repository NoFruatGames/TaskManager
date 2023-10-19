using TransferDataTypes;

namespace TMServer.ConnectionTools.Connections
{
    internal interface IConnection : IDisposable
    {
        public bool IsConnected { get; }
        public event Action<IConnection>? ClientDisconnected;
        public string Id { get; }

        string GetRemoteHost();
        int GetRemotePort();
        void Close();

        TMMessage? GetMessage();
        Task<TMMessage?> GetMessageAsync();

        void SendMessage(TMMessage message);
        Task SendMessageAsync(TMMessage message);
    }
}
