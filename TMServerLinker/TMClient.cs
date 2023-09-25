using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransferDataTypes;
using TransferDataTypes.Payloads;
using TMServerLinker.ConnectionHandlers;
namespace TMServerLinker
{
    public class TMClient :IDisposable
    {
        private ConnectionHandler connection;
        Thread connectionThread;
        private static int id;
        private string? DataFolder { get; init; }
        private string? sessionKey = null; 
        public TMClient(ConnectionHandler connection)
        {
            this.connection = connection;
            connection.UpdateMessageReceived += Connection_UpdateMessageReceived;
            connection.ResponseMessageReceived += Connection_ResponseMessageReceived;
            connection.OnConnectionOpened += Connection_OnConnectionOpened;
            connectionThread = new Thread(()=>connection.ConnectToServerAsync());
            connectionThread.Name = "connection thread";
            connectionThread.Start();
        }
        public TMClient(ConnectionHandler connection, string dataFolder) :this(connection)
        {
            DataFolder = dataFolder;
        }
        private void GetSessionKeyFromFile()
        {
            if (DataFolder == null) return;
            try
            {
                Directory.CreateDirectory(DataFolder);
                string file = Path.Combine(DataFolder, "session.key");
                using (var stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    using(var reader = new StreamReader(stream))
                    {
                        sessionKey = reader.ReadToEnd();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Read session key error: {ex}");
            }
        }
        private void SaveSessionKeyToFile()
        {
            if (DataFolder == null) return;
            try
            {
                Directory.CreateDirectory(DataFolder);
                string file = Path.Combine(DataFolder, "session.key");
                using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(sessionKey ?? string.Empty);
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Write session key error: {ex}");
            }
        }

        private void Connection_OnConnectionOpened()
        {
            GetSessionKeyFromFile();
            Message message = new Message()
            { 
                Action = MessageAction.InitSession, 
                Type=MessageType.Request,
                Payload=sessionKey,
                Id = id
            };
            id += 1;
            connection.AddMessageToQueue(message);
        }

        List<Message> responses = new List<Message>();
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
            if(response is not null)
            {
                PayloadCreateAccountResult? result = response.DeserializePayload<PayloadCreateAccountResult>();
                if(result is not null)
                {
                    sessionKey = result.Value.SessionKey;
                    SaveSessionKeyToFile();
                    Console.WriteLine($"{sessionKey}\t{result.Value.Result.ToString()}");
                }

            }
            else
            {
                Console.WriteLine("timeout exceeded");
            }

        }
        public void LoginToAccount(Profile profile)
        {
            ++id;
            Message message = new Message()
            {
                Type = MessageType.Request,
                Action = MessageAction.LoginToAccount,
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
            if (response is not null)
            {
                PayloadLoginToAccountResult? loginResult = response?.DeserializePayload<PayloadLoginToAccountResult>();
                if(loginResult is not null)
                {
                    sessionKey = loginResult.Value.SessionKey;
                    SaveSessionKeyToFile();
                    Console.WriteLine($"{sessionKey}\t{loginResult.Value.Result.ToString()}");
                }
                
            }
            else
            {
                Console.WriteLine("timeout exceeded");
            }
        }
    }
}
