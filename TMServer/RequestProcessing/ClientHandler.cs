using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using TMServer.ConnectionTools.ConnectionProcessors;
using TransferDataTypes;

namespace TMServer.RequestProcessing
{
    internal class ClientHandler :IDisposable
    {
        private IConnectionProcessor Connection { get;}
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Host { get; }
        public int Port { get; }


        public delegate Task MessageWithSender(Message message, ClientHandler connectionHandler);
        public event Action<ClientHandler>? ClientDisconnected;
        public event MessageWithSender? OnMessageRecived;
        public ClientHandler(IConnectionProcessor Connection)
        {
            this.Connection = Connection;
            Host = Connection.GetHost();
            Port = Connection.GetPort();
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Connection open with {Connection.GetHost()}:{Connection.GetPort()}");
        }
        public void Dispose()
        {
            Connection.Dispose();
        }
        public async Task HandleClientAsync()
        {
            Thread.CurrentThread.Name = Id + " client handler";
            try
            {
                while (true)
                {

                    string? json = await Connection.ReadLineAsync();
                    Message? message = Message.Deserialize(json ?? string.Empty);
                    if (message != null)
                    {
                        if (message.Type == MessageType.Request && OnMessageRecived is not null)
                            await OnMessageRecived.Invoke(message, this);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                ClientDisconnected?.Invoke(this);
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Client {Id} has disconnected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Client {Id} error: {ex.Message}");
            }
        }
        public async Task SendMessageAsync(string message, bool flush = true)
        {
            await Connection.WriteLineAsync(message, flush);
        }
        public override string ToString()
        {
            return $"{Host}:{Port}\t{Id}";
        }
    }
}
