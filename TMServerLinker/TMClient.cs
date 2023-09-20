using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferDataTypes;
using TransferDataTypes.Payloads;
namespace TMServerLinker
{
    public class TMClient :IDisposable
    {
        ConnectionHandler connection;
        Thread connectionThread;
        private static int id = 0;
        public TMClient(string serverHost="192.168.0.129", int serverPort = 4980)
        {
            responses = new List<Message> ();
            connection = new ConnectionHandler(serverHost, serverPort);
            connection.UpdateMessageReceived += Connection_UpdateMessageReceived;
            connection.ResponseMessageReceived += Connection_ResponseMessageReceived;
            connectionThread = new Thread(()=>connection.ConnectToServerAsync());
            connectionThread.Name = "connection thread";
            connectionThread.Start();
        }
        List<Message> responses;
        //Блокирующий метод
        private Message? GetResponseById(int id, int waitingSeconds= 15)
        {
            int failCount = 0;
            while (true)
            {
                if (failCount >= waitingSeconds * 2 && waitingSeconds > 0)
                    return null;
                for(int i = 0; i < responses.Count; ++i)
                {
                    if (responses[i].Id == id)
                    {
                        return responses[i];
                    }
                }
                if(waitingSeconds > 0)
                {
                    failCount += 1;
                    Thread.Sleep(500);
                }

            }
        }
        private void Connection_ResponseMessageReceived(Message obj)
        {
            responses.Add(obj);
        }

        private void Connection_UpdateMessageReceived(Message obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            try
            {
                connection.Dispose();
            }
            catch{}

        }
        public void RegisterAccount(Profile profile)
        {
            ++id;
            DateTime sendTime = DateTime.Now;
            Message message = new Message()
            {
                Type = MessageType.Request,
                Action = MessageAction.RegisterAccount,
                Id = id,
                Payload = new PayloadAccountInfo()
                {
                    Username = profile.Username,
                    Password = profile.Password,
                    Email = profile.Email,
                }
            };
            connection.AddMessageToQueue(message);
            Message? response = GetResponseById(message.Id, -1);
            if(response != null)
            {
                Console.WriteLine(response?.DeserializePayload<PayloadCreateAccountResult>().ToString());
            }
            else
            {
                Console.WriteLine("timeout exceeded");
            }

        }
    }
}
