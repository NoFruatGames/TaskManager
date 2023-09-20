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
        private string id;
        private string clientIP=string.Empty;
        private int clientPort=-1;
        private readonly object locker = new object();

        public delegate R RequestWithResponse<P, R>(P payload);
        public event Action<ClientHandler>? ClientDisconnected;
        public event RequestWithResponse<PayloadAccountInfo?, CreateAccountResult>? CreateAccountRequest;
        

        public ClientHandler(TcpClient client) { 
            this.client = client;
            stream = client.GetStream();
            this.writer = new StreamWriter(client.GetStream());
            this.reader = new StreamReader(client.GetStream());
            id = Guid.NewGuid().ToString();
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
            Thread.CurrentThread.Name = id + " client handler";
            try
            {
                while (true)
                {

                    string? json = await reader.ReadLineAsync();
                    Message? message = Message.Deserialize(json ?? string.Empty);
                    if (message != null)
                    {
                        if (message.Action == MessageAction.RegisterAccount && message.Type == MessageType.Request && message.Payload != null)
                        {
                            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Create account request from client {id}");
                            PayloadAccountInfo? accountInfo = message.DeserializePayload<PayloadAccountInfo>();
                            CreateAccountResult? result = CreateAccountRequest?.Invoke(accountInfo);
                            if (result != null)
                            {
                                Message response = new Message()
                                {
                                    Action = MessageAction.RegisterAccount,
                                    Type = MessageType.Response,
                                    Id = message.Id,
                                };
                                if (result == CreateAccountResult.Success) response.Payload = PayloadCreateAccountResult.Success;
                                string responseJson = response.Serialize();
                                await SendMessageAsync(responseJson);
                                
                            }
                        }
                    }
                }
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex.Message);
                ClientDisconnected?.Invoke(this);
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Client {id} has disconnected");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Client {id} error: {ex.Message}");
            }
        }
        private async Task SendMessageAsync(string  message)
        {
            try
            {
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
            }
            catch(IOException)
            {
                Dispose();
                ClientDisconnected?.Invoke(this);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: sending data to client {id} error: {ex.Message}");
            }

        }
        public override string ToString()
        {
            return $"{clientIP}:{clientPort}\t{id}";
        }
    }
    
    internal enum CreateAccountResult
    {
        Success
    };
}
