using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using TransferDataTypes.Payloads;
using TransferDataTypes;

namespace TMServer
{
    internal class ClientHandler :IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;
        public string Id { get; init; }=string.Empty;
        private string clientIP=string.Empty;
        private int clientPort=-1;
        private readonly object locker = new object();

        public delegate Task MessageWithSender(Message message,ClientHandler sender);
        public event Action<ClientHandler>? ClientDisconnected;
        public event MessageWithSender? OnRequestRecived;

        public ClientHandler(TcpClient client) { 
            this.client = client;
            stream = client.GetStream();
            this.writer = new StreamWriter(client.GetStream());
            this.reader = new StreamReader(client.GetStream());
            Id = Guid.NewGuid().ToString();
            if(client.Client.RemoteEndPoint is IPEndPoint endPoint)
            {
                clientIP = endPoint.Address.ToString();
                clientPort = endPoint.Port;
            }
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Connection open with {clientIP}:{clientPort}");
        }
        public void Dispose()
        {
            try
            {
                lock (locker)
                {
                    client.Close();
                    client.Dispose();
                    stream.Close();
                    stream.Dispose();
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch { }
            

        }
        public async Task HandleClient()
        {
            Thread.CurrentThread.Name = Id + " client handler";
            try
            {
                while (true)
                {

                    string? json = await reader.ReadLineAsync();
                    Message? message = Message.Deserialize(json ?? string.Empty);
                    if (message != null)
                    {
                        if (message.Type == MessageType.Request && OnRequestRecived is not null)
                        {
                            await OnRequestRecived.Invoke(message, this);
                        }
                    }
                }
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex.Message);
                ClientDisconnected?.Invoke(this);
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Client {Id} has disconnected");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Client {Id} error: {ex.Message}");
            }
        }
        public async Task SendMessageAsync(string  message)
        {
            try
            {
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
            catch(IOException)
            {
                ClientDisconnected?.Invoke(this);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: sending data to client {Id} error: {ex.Message}");
            }

        }
        public override string ToString()
        {
            return $"{clientIP}:{clientPort}\t{Id}";
        }
    }
    

}
