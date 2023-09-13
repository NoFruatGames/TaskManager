using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using TransferDataTypes.Payloads;
using TransferDataTypes;
using Newtonsoft.Json;

namespace TMServer
{
    internal class ClientHandler :IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;
        private string id;
        private string clientIP=string.Empty;
        private int clientPort=-1;
        private readonly object locker = new object();

        public delegate R RequestWithResponse<P, R>(P payload);
        public event Action<ClientHandler>? ClientDisconnected;
        public event RequestWithResponse<PayloadAccountInfo, CreateAccountResult>? CreateAccountRequest;
        

        public ClientHandler(TcpClient client) { 
            this.client = client;
            stream = client.GetStream();
            this.writer = new StreamWriter(client.GetStream());
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
            lock(locker)
            {
                client.Close();
                client.Dispose();
                stream.Close();
                stream.Dispose();
                writer.Close();
                writer.Dispose();
            }

        }
        public void HandleClient()
        {
            Thread.CurrentThread.Name = id + " client handler";
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Message? message = JsonConvert.DeserializeObject<Message>(json);
                    if (message != null)
                    {
                        if (message.Action == MessageAction.RegisterAccount && message.Type==MessageType.Request && message.Payload != null)
                        {
                            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Create account request from client {id}");
                            message.Payload = JsonConvert.DeserializeObject<PayloadAccountInfo>(message.Payload.ToString());
                            CreateAccountResult? result = CreateAccountRequest?.Invoke((PayloadAccountInfo)message.Payload);
                            if (result != null)
                            {
                                Message response = new Message()
                                {
                                    Action = MessageAction.RegisterAccount,
                                    Type = MessageType.Response
                                };
                                if (result == CreateAccountResult.Success) response.Payload = PayloadCreateAccountResult.Success;
                                string jsonResult = JsonConvert.SerializeObject(response);
                                SendMessage(jsonResult);
                            }
                        }
                    }
                }
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex.Message);
                Dispose();
                ClientDisconnected?.Invoke(this);
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Client {id} has disconnected");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Client {id} error: {ex.Message}");
            }
        }
        private void SendMessage(string message)
        {
            try
            {
                writer.Write(message);
                writer.Flush();
            }catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: sending data to client {id} error: {ex.Message}");
                Dispose();
                ClientDisconnected?.Invoke(this);

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
