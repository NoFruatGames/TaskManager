using TMServerLinker.ConnectionHandlers;
using TransferDataTypes;
using TransferDataTypes.Messages;

namespace TMServerLinker
{
    public class TMClient : IDisposable
    {
        private ConnectionHandler connection;
        private Thread connectionThread;
        private static int requestId;
        private string? DataFolder { get; init; }
        private string? sessionKey = null;
        public bool Autorized { get; private set; } = false;
        private TMClient(ConnectionHandler connection)
        {
            this.connection = connection;
            connection.MessageRecived += Connection_MessageRecived;
            connection.OnConnectionOpened += Connection_OnConnectionOpened;
            connectionThread = new Thread(() => connection.ConnectToServerAsync());
            connectionThread.Name = "connection thread";
            connectionThread.Start();
        }
        private void Connection_MessageRecived(TMMessage obj)
        {
            Console.WriteLine(obj.ToString());
            responses.Add(obj);
        }

        public TMClient(ConnectionHandler connection, string dataFolder) : this(connection)
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
                    using (var reader = new StreamReader(stream))
                    {
                        sessionKey = reader.ReadToEnd();
                        sessionKey = sessionKey.Trim();
                    }
                }
            }
            catch (Exception ex)
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Write session key error: {ex}");
            }
        }

        private void Connection_OnConnectionOpened()
        {
            GetSessionKeyFromFile();
        }

        private List<TMMessage> responses = new List<TMMessage>();
        //Блокирующий метод
        //private TMMessage? GetResponseById(int id, int waitingSeconds = 15)
        //{
        //    int failCount = 0;
        //    while (true)
        //    {
        //        if (failCount >= waitingSeconds * 2 && waitingSeconds > 0)
        //            return null;
        //        for (int i = 0; i < responses.Count; ++i)
        //        {
        //            if (responses[i].Id == id)
        //            {
        //                return responses[i];
        //            }
        //        }
        //        if (waitingSeconds > 0)
        //        {
        //            failCount += 1;
        //            Thread.Sleep(500);
        //        }

        //    }
        //}
        public void Dispose()
        {
            try
            {
                connection.Dispose();
            }
            catch { }

        }
        public void RegisterAccount(Profile profile)
        {
            CreateAccountMessage request = new CreateAccountMessage() 
            {
                Username = profile.Username,
                Password = profile.Password,
                Email = profile.Email,
                IsRequest=true,
                SessionToken = sessionKey ?? string.Empty
            };
            connection.AddMessageToQueue(request);

        }
        public void LoginToAccount(Profile profile)
        {

        }
        public void Logout()
        {
            LogoutMessage message = new LogoutMessage()
            { 
                SessionToken = sessionKey ?? string.Empty,
                IsRequest = true,
            };
            connection.AddMessageToQueue(message);
        }
    }
}
 