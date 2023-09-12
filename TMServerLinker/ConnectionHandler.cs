using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TMServerLinker
{
    internal class ConnectionHandler :IDisposable
    {
        public event Action<string>? OnMessageReceived;
        private TcpClient? Server { get; set; }
        private StreamReader? Reader { get; set; }
        private StreamWriter? Writer { get; set; }
        private readonly string ServerHost;
        private readonly int ServerPort;
        private readonly object locker = new object();
        public bool IsConnected { get; private set; } = false;
        Thread reciveThread;
        public ConnectionHandler(string serverHost = "192.168.0.129", int serverPort = 4980)
        {
            ServerHost = serverHost;
            ServerPort = serverPort;
        }

        public void ConnectToServer()
        {
            while (true)
            {
                try
                {
                    if (Server != null && Server.Connected == true && IsConnected == true) continue;
                    Dispose();
                    IsConnected = false;
                    Server = new TcpClient();
                    Server.Connect(ServerHost, ServerPort);
                    lock (locker)
                    {
                        Reader = new StreamReader(Server.GetStream());
                        Writer = new StreamWriter(Server.GetStream());
                        IsConnected = true;
                    }
                    reciveThread = new Thread(StartReceivingData);
                    reciveThread.Name = "Recive from server thread";
                    reciveThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection failed: {ex.Message}");
                    Dispose();
                    Task.Delay(TimeSpan.FromSeconds(5)); // Пауза перед повторной попыткой подключения
                }
            }
        }
        private void StartReceivingData()
        {
            try
            {
                while (IsConnected && Server != null && Server.Connected && Reader != null)
                {
                    string? json = Reader.ReadLine();
                    if (json != null)
                    {
                        //Message? message = JsonConvert.DeserializeObject<Message>(json);
                        //if (message != null) OnMessageReceived?.Invoke(message);
                        Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Message from server: {json}");
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
            Server?.Close();
            Server?.Dispose();
            Reader?.Close();
            Reader?.Dispose();
            Writer?.Close();
            Writer?.Dispose();
        }
    }
}
