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
    internal class ConnectionHandler : IDisposable
    {
        private TcpClient? Server { get; set; }
        private NetworkStream Stream { get; set; }
        private StreamWriter? Writer { get; set; }
        private Queue<Message> Requests { get; set; }
        private List<RequestResponse> Responses{get;set; }
        private readonly string ServerHost;
        private readonly int ServerPort;
        private readonly object locker = new object();
        private bool MessageProcessed { get; set; } = true;
        public bool IsConnected { get; private set; } = false;
        Thread reciveThread;
        Thread sendThread;
        public ConnectionHandler(string serverHost = "192.168.0.129", int serverPort = 4980)
        {
            ServerHost = serverHost;
            ServerPort = serverPort;
            Requests = new Queue<Message>();
            Responses = new List<RequestResponse>();
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
                    MessageProcessed = true;
                    Server = new TcpClient();
                    await Server.ConnectAsync(ServerHost, ServerPort);
                    lock (locker)
                    {
                        Stream = Server.GetStream();
                        Writer = new StreamWriter(Server.GetStream());
                        IsConnected = true;
                    }
                    reciveThread = new Thread(()=>StartReceivingDataAsync());
                    reciveThread.Name = "Recive from server thread";
                    sendThread = new Thread(() => SendRequestFromQueue());
                    sendThread.Name = "Send to server thread";
                    reciveThread.Start();
                    sendThread.Start();
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
                byte[] buffer = new byte[1024];
                int bytesRead;
                
                while ((bytesRead = await Stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (json != null)
                    {
                        Message message = JsonConvert.DeserializeObject<Message>(json);
                        if(message.Type == MessageType.Response)
                        {
                            for(int i = 0; i < Responses.Count; i++)
                            {
                                if (Responses[i].Response == null)
                                {
                                    RequestResponse newRequest = new RequestResponse()
                                    {
                                        Request = Responses[i].Request,
                                        Response = message
                                    };
                                    Responses[i] = newRequest;
                                    MessageProcessed = true;
                                    break;
                                }
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
                }
            }
            catch{}
            

        }
        private async Task SendRequestFromQueue()
        {
            try
            {
                while (true)
                {

                    if (Requests.Count - 1 >= 0 && MessageProcessed)
                    {
                        MessageProcessed = false;
                        Message request = Requests.Dequeue();
                        string json = JsonConvert.SerializeObject(request);
                        await Writer?.WriteAsync(json);
                        await Writer?.FlushAsync();
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Writting data error: {ex.Message}");
            }
            
            

        }
        ///Блокирующий вызов
        public Message GetResponseFromRequest(Message request)
        {
            while (true)
            {
                for(int i = 0; i < Responses.Count; ++i)
                {
                    if (Responses[i].Request == request && Responses[i].Response != null)
                    {
                        RequestResponse response = Responses[i];
                        Responses.Remove(response);
                        return response.Response;
                    }
                }
            }
        }
        public void AddMessageToQueue(Message message)
        {
            Requests.Enqueue(message);
            Responses.Add(new RequestResponse() { Request = message });
        }
        private struct RequestResponse
        {
            public Message Request { get; set; }
            public Message Response { get; set; }
        }
    }
}
