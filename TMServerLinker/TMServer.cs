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
            connection.OnMessageReceived += Connection_OnMessageReceived;
            connectionThread = new Thread(connection.ConnectToServer);
            connectionThread.Name = "connection thread";
            connectionThread.Start();
        }

        private void Connection_OnMessageReceived(string json)
        {
            Message? message = JsonConvert.DeserializeObject<Message>(json);
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Message from server: {json}");
        }

        public void Dispose()
        {
            connection.Dispose();
        }
        public void RegisterAccount(Profile profile)
        {
            Message message = new Message()
            {
                Type = MessageType.RegisterAccount,
                Payload = new PayloadAccountInfo()
                {
                    Username = profile.Username,
                    Password = profile.Password,
                    Email = profile.Email,
                }
            };
            string json = JsonConvert.SerializeObject(message);
            Task.Factory.StartNew(() => connection.SendMessage(json));
            
           // _ = Task.Run(() => );
        }
    }
}
