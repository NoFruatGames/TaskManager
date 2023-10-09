using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferDataTypes;

namespace TMServer.ConnectionTools.Connections
{
    internal interface IConnection : IDisposable
    {
        public bool IsConnected { get; }
        public event Action<IConnection>? ClientDisconnected;


        string GetRemoteHost();
        int GetRemotePort();
        void Close();

        TMMessage? GetMessage();
        Task<TMMessage?> GetMessageAsync();

        void SendMessage(TMMessage message);
        Task SendMessageAsync(TMMessage message);
    }
}
