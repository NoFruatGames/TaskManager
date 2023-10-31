using System.Net.Sockets;
using TransferDataTypes;

namespace TMServerLinker.ConnectionHandlers
{
    public class TCPConnectionHandler : ConnectionHandler
    {
        private TcpClient Server { get; set; } = null!;
        private StreamWriter Writer { get; set; } = null!;
        private StreamReader Reader { get; set; } = null!;

        internal override event Action<TMMessage>? MessageRecived;
        internal override event Action? OnConnectionOpened;
        internal override event Action? OnConnectionClosed;

        public TCPConnectionHandler(string serverHost = "192.168.0.129", int serverPort = 4980) : base(serverHost, serverPort)
        {
            
        }

        internal override async Task ConnectToServerAsync()
        {
            while (NeedToReconnect)
            {
                try
                {
                    if (Server is not null && Server.Connected == true && IsConnected == true) continue;
                    Dispose();
                    Server = new TcpClient();
                    OnConnectionClosed?.Invoke();
                    await Server.ConnectAsync(ServerHost, ServerPort);
                    lock (locker)
                    {
                        Writer = new StreamWriter(Server.GetStream());
                        Reader = new StreamReader(Server.GetStream());
                        IsConnected = true;
                        reciveThread = new Thread(() => StartReceivingDataAsync());
                        reciveThread.Name = "Recive from server thread";
                        sendThread = new Thread(() => SendRequestFromQueueAsync());
                        sendThread.Name = "Send to server thread";
                        reciveThread.Start();
                        sendThread.Start();
                        OnConnectionOpened?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection failed: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(5)); // Пауза перед повторной попыткой подключения
                }
            }
            Dispose();
        }
        private async Task StartReceivingDataAsync()
        {
            try
            {
                while (IsConnected)
                {
                    string? json = await Reader.ReadLineAsync();
                    TMMessage? message = TMMessage.Deserialize(json ?? string.Empty);
                    if (message is not null) MessageRecived?.Invoke(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data from server: {ex.Message}");
            }
        }
        private async Task SendRequestFromQueueAsync()
        {
            try
            {
                while (IsConnected)
                {
                    if (Requests.Count - 1 >= 0)
                    {
                        TMMessage request = Requests.Dequeue();
                        await Writer.WriteLineAsync(request.Serialize());
                        await Writer.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"sending to server error: {ex.Message}");
            }

        }



        public override void Dispose()
        {
            try
            {
                lock (locker)
                {
                    IsConnected = false;
                    Server?.Close();
                    Server?.Dispose();
                    Writer?.Close();
                    Writer?.Dispose();
                    Reader?.Close();
                    Reader?.Dispose();
                }
            }
            catch { }
        }
    }
}
