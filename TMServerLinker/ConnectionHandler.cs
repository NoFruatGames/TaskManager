using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TransferDataTypes.Payloads;
using TransferDataTypes;

namespace TMServerLinker
{
    internal class ConnectionHandler :IDisposable
    {
        public event Action<string>? OnMessageReceived;
        private TcpClient? Server { get; set; }
        private NetworkStream Stream { get; set; }
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
                        Stream = Server.GetStream();
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
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = Stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (json != null)
                    {
                        OnMessageReceived?.Invoke(json);
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
            lock (locker)
            {
                Server?.Close();
                Server?.Dispose();
                Stream?.Close();
                Stream?.Dispose();
                Writer?.Close();
                Writer?.Dispose();
            }

        }
        public void SendMessage(string message)
        {
            try
            {
                Writer?.Write(message);
                Writer?.Flush();
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Writting data error: {ex.Message}");
            }
        }
    }
}
