using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TransferDataTypes;

namespace TMServerLinker.ConnectionHandlers
{
    public class TCPConnectionHandler : ConnectionHandler
    {
        private TcpClient? Server { get; set; }
        private NetworkStream Stream { get; set; } = null!;
        private StreamWriter Writer { get; set; } = null!;
        private StreamReader Reader { get; set; } = null!;

        internal override event Action<Message>? UpdateMessageRecived;
        internal override event Action<Message>? ResponseMessageRecived;
        internal override event Action<bool>? InitSessionMessageRecived;
        internal override event Action? OnConnectionOpened;
        internal override event Action? OnConnectionClosed;

        public TCPConnectionHandler(string serverHost = "192.168.0.129", int serverPort = 4980):base(serverHost, serverPort)
        {

        }

        internal override async Task ConnectToServerAsync()
        {
            while (true)
            {
                try
                {
                    if (Server != null && Server.Connected == true && IsConnected == true) continue;
                    Dispose();
                    IsConnected = false;
                    OnConnectionClosed?.Invoke();
                    Server = new TcpClient();
                    await Server.ConnectAsync(ServerHost, ServerPort);
                    lock (locker)
                    {
                        Stream = Server.GetStream();
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
                    Dispose();
                    await Task.Delay(TimeSpan.FromSeconds(5)); // Пауза перед повторной попыткой подключения
                }
            }
        }
        private async Task StartReceivingDataAsync()
        {
            try
            {
                while (true)
                {
                    string? json = await Reader.ReadLineAsync();
                    if (json != null)
                    {
                        Message? message = Message.Deserialize(json);
                        if (message != null)
                        {
                            if (message.Type == MessageType.None || message.Action == MessageAction.None) continue;
                            if (message.Type == MessageType.Response && message.Action != MessageAction.InitSession) ResponseMessageRecived?.Invoke(message);
                            else if (message.Type == MessageType.Update) UpdateMessageRecived?.Invoke(message);
                            else if (message.Type == MessageType.Response && message.Action == MessageAction.InitSession) InitSessionMessageRecived?.Invoke(message.DeserializePayload<bool>());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data from server: {ex.Message}");
            }
        }
        private async Task SendRequestFromQueueAsync()
        {

        }



        public override void Dispose()
        {
            try
            {
                lock (locker)
                {
                    Server?.Close();
                    Server?.Dispose();
                    Stream?.Close();
                    Stream?.Dispose();
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
