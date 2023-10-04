using System;
using System.Net;
using System.Net.Sockets;

namespace TMServer.ConnectionTools.ConnectionProcessors
{
    internal class TCPConnectionProcessor : IConnectionProcessor
    {
        TcpClient Connection { get; init; }
        StreamReader Reader { get; init; }
        StreamWriter Writer { get; init; }

        public TCPConnectionProcessor(TcpClient client)
        {
            Connection = client;
            Reader = new StreamReader(client.GetStream());
            Writer = new StreamWriter(client.GetStream());
        }
        public void Dispose()
        {
            try
            {
                Connection.Close();
                Connection.Dispose();
            }
            catch { }
        }
        public string GetHost()
        {
            if (Connection.Client.RemoteEndPoint is IPEndPoint ep)
            {
                return ep.Address.ToString();
            }
            return string.Empty;
        }
        public int GetPort()
        {
            if (Connection.Client.RemoteEndPoint is IPEndPoint ep)
            {
                return ep.Port;
            }
            return -1;
        }
        public async Task<string?> ReadLineAsync()
        {
            try
            {
                return await Reader.ReadLineAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task WriteLineAsync(string message, bool flush = true)
        {
            try
            {
                await Writer.WriteLineAsync(message);
                if (flush) await Writer.FlushAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
