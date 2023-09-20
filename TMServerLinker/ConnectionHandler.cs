using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using TransferDataTypes.Payloads;
using TransferDataTypes;
using System.Threading;

namespace TMServerLinker
{
    internal class ConnectionHandler : IDisposable
    {
        private TcpClient? Server { get; set; }
        private NetworkStream Stream { get; set; } = null!;
        private StreamWriter Writer { get; set; } = null!;
        private StreamReader Reader { get; set; } = null!; 
        private Queue<Message> Requests { get; set; }
        private readonly string ServerHost;
        private readonly int ServerPort;
        private readonly object locker = new object();
        public bool IsConnected { get; private set; } = false;
        Thread reciveThread = null!;
        Thread sendThread = null!;

        public event Action<Message>? UpdateMessageReceived;
        public event Action<Message>? ResponseMessageReceived;



        public ConnectionHandler(string serverHost = "192.168.0.129", int serverPort = 4980)
        {
            ServerHost = serverHost;
            ServerPort = serverPort;
            Requests = new Queue<Message>();
        }

        public async Task ConnectToServerAsync()
        {
            while (true)
            {
                try
                {
                    if (Server != null && Server.Connected == true && IsConnected == true) continue;
                    Dispose();
                    IsConnected = false;
                    Server = new TcpClient();
                    await Server.ConnectAsync(ServerHost, ServerPort);
                    lock (locker)
                    {
                        Stream = Server.GetStream();
                        Writer = new StreamWriter(Server.GetStream());
                        Reader = new StreamReader(Server.GetStream());
                        IsConnected = true;
                        reciveThread = new Thread(()=> StartReceivingDataAsync());
                        reciveThread.Name = "Recive from server thread";
                        sendThread = new Thread(()=>SendRequestFromQueueAsync());
                        sendThread.Name = "Send to server thread";
                        reciveThread.Start();
                        sendThread.Start();
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
        private  async Task StartReceivingDataAsync() 
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
                            if (message.Type == MessageType.Response && ResponseMessageReceived != null) ResponseMessageReceived.Invoke(message);
                            else if (message.Type == MessageType.UpdateClient && UpdateMessageReceived != null)
                            {
                                Thread updateThread = new Thread(() => UpdateMessageReceived.Invoke(message));
                                updateThread.Name = $"update message {message.Id} handler";
                                updateThread.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving data from server: {ex.Message}");
            }
        }

        public void Dispose()
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
            catch{}
            

        }
        private async Task SendRequestFromQueueAsync()
        {
            try
            {
                while (true)
                {

                    if (Requests.Count - 1 >= 0)
                    {
                        Message request = Requests.Dequeue();
                       string json = request.Serialize();
                        if (!string.IsNullOrEmpty(json))
                        {
                            await Writer?.WriteLineAsync(json);
                            await Writer?.FlushAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Writting data error: {ex.Message}");
            }
        }
        public void AddMessageToQueue(Message message)
        {
            Requests.Enqueue(message);
        }
    }
}
