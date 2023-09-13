using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferDataTypes;
using TransferDataTypes.Payloads;
namespace TMServerLinker
{
    public class TMServer :IDisposable
    {
        ConnectionHandler connection;
        Thread connectionThread;
        public TMServer(string serverHost="192.168.0.129", int serverPort = 4980)
        {
            connection = new ConnectionHandler(serverHost, serverPort);
            connectionThread = new Thread(()=>connection.ConnectToServerAsync());
            connectionThread.Name = "connection thread";
            connectionThread.Start();
        }


        public void Dispose()
        {
            connection.Dispose();
        }
        public void RegisterAccount(Profile profile)
        {
            Message message = new Message()
            {
                Type = MessageType.Request,
                Action = MessageAction.RegisterAccount,
                Payload = new PayloadAccountInfo()
                {
                    Username = profile.Username,
                    Password = profile.Password,
                    Email = profile.Email,
                }
            };
            connection.AddMessageToQueue(message);
            Message response = connection.GetResponseFromRequest(message); //блокирующий вызов
            Console.WriteLine(response.Payload.ToString());
        }
    }
}
