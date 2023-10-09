using System.Net;
using System.Net.Sockets;
using TransferDataTypes;

namespace TMServer.ConnectionTools.Connections
{
    internal class TCPConnection : IConnection
    {
        public bool IsConnected { get; private set; } = true;
        public event Action<IConnection>? ClientDisconnected;

        private TcpClient Connection { get; init; }
        private StreamReader Reader { get; init; }
        private StreamWriter Writer { get; init; }


        public string GetRemoteHost()
        {
            if (Connection.Client.RemoteEndPoint is IPEndPoint ep)
                return ep.Address.ToString();
            else
                return string.Empty;

        }
        public int GetRemotePort()
        {
            if (Connection.Client.RemoteEndPoint is IPEndPoint ep)
                return ep.Port;
            else
                return -1;
        }
        public void Close()
        {
            ClientDisconnected?.Invoke(this);
        }

        public TMMessage? GetMessage()
        {
            if (IsConnected == false) return null;
            if (!Connection.Connected) { ClientDisconnected?.Invoke(this); return null; }
            try
            {
                TMMessage? message = null;
                string? text = null;
                do
                {
                    text = Reader.ReadLine();
                    message = TMMessage.Deserialize(text ?? string.Empty);
                } while (message is null);

                return message;
            }
            catch (IOException)
            {
                ClientDisconnected?.Invoke(this);
            }
            return null;
        }
        public async Task<TMMessage?> GetMessageAsync()
        {
            if (IsConnected == false) return null;
            if (!Connection.Connected) { ClientDisconnected?.Invoke(this); return null; }
            try
            {
                TMMessage? message = null;
                string? text = null;
                do
                {
                    text = await Reader.ReadLineAsync();
                    message = TMMessage.Deserialize(text ?? string.Empty);
                } while (message is null);

                return message;
            }
            catch (IOException)
            {
                ClientDisconnected?.Invoke(this);
            }
            return null;
        }

        public void SendMessage(TMMessage message)
        {
            if (IsConnected == false) return;
            if (!Connection.Connected) { ClientDisconnected?.Invoke(this); return; }
            try
            {
                Writer.WriteLine(message.Serialize());
                Writer.Flush();
            }
            catch (IOException)
            {
                ClientDisconnected?.Invoke(this);
            }

        }
        public async Task SendMessageAsync(TMMessage message)
        {
            if (IsConnected == false) return;
            if (!Connection.Connected) { ClientDisconnected?.Invoke(this); return; }
            try
            {
                await Writer.WriteLineAsync(message.Serialize());
                await Writer.FlushAsync();
            }
            catch (IOException)
            {
                ClientDisconnected?.Invoke(this);
            }
        }


        public void Dispose()
        {
            try
            {
                Reader?.Close();
                Writer?.Close();
                Connection?.Close();
                Connection?.Dispose();
                Reader?.Dispose();
                Writer?.Dispose();
            }
            catch { }
        }
        public TCPConnection(TcpClient connection)
        {
            Connection = connection;
            Reader = new StreamReader(connection.GetStream());
            Writer = new StreamWriter(connection.GetStream());
            ClientDisconnected += ClientDisconnectedHandler;
        }

        private void ClientDisconnectedHandler(IConnection connection)
        {
            IsConnected = false;
            Dispose();
        }

    }
}
