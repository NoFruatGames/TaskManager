using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServerLinker
{
    public class TMServer :IDisposable
    {
        ConnectionHandler connection;
        Thread connectionThread;
        public TMServer(string serverHost="192.168.0.129", int serverPort = 4980)
        {
            connection = new ConnectionHandler(serverHost, serverPort);
            connectionThread = new Thread(connection.ConnectToServer);
            connectionThread.Name = "connection thread";
            connectionThread.Start();
        }
        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
