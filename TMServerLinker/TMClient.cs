using TMServerLinker.ConnectionHandlers;
using TransferDataTypes;
using TransferDataTypes.Messages;
using TransferDataTypes.Results;
namespace TMServerLinker
{
    public class TMClient : IDisposable
    {
        private ConnectionHandler connection;

        private static int requestId;
        private string? DataFolder { get; init; }
        private string? sessionKey = null;
        public bool Autorized { get; private set; } = false;


        private bool LogoutExecuting = false;



        private TMClient(ConnectionHandler connection)
        {
            this.connection = connection;
            connection.MessageRecived += HandleReceivedMessage;
            connection.OnConnectionOpened += Connection_OnConnectionOpened;
            Thread connectionThread = new Thread(() => connection.ConnectToServerAsync());
            connectionThread.Name = "connection thread";
            connectionThread.Start();
        }
        private void HandleReceivedMessage(TMMessage obj)
        {
            Console.WriteLine(obj);
            if (CheckSessionMessage.IsIdentity(obj))
            {
                CheckSessionMessage? message = CheckSessionMessage.TryParse(obj);
                if (message is not null && message.CheckTokenResult == CheckSessionResult.Success)
                    Autorized = true;
                else
                    Autorized = false;
            }
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
            CheckSessionMessage check = new CheckSessionMessage()
            {
                IsRequest = true,
                SessionToken = string.IsNullOrEmpty(sessionKey) ? "none" : sessionKey
            };
            connection.AddMessageToQueue(check);
        }

        private Queue<TMMessage> responses = new Queue<TMMessage>();
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
                SessionToken = string.IsNullOrEmpty(sessionKey) ? "none" : sessionKey
            };
            connection.AddMessageToQueue(request);

        }
        public void LoginToAccount(Profile profile)
        {

        }
        public async Task<bool> Logout()
        {
            if(LogoutExecuting || !Autorized) return false;
            LogoutMessage message = new LogoutMessage()
            { 
                SessionToken =  string.IsNullOrEmpty(sessionKey)?"none":sessionKey,
                IsRequest = true,
            };
            connection.AddMessageToQueue(message);
            LogoutExecuting = true;
            var tcs = new TaskCompletionSource<bool>();
            Action<TMMessage> messageReceivedHandler = null;
            messageReceivedHandler = (response) =>
            {
                if (LogoutMessage.IsIdentity(response))
                {
                    LogoutMessage? logoutMessage = LogoutMessage.TryParse(response);
                    if (logoutMessage is not null && logoutMessage.LogoutResult == LogoutResult.Success)
                    {
                        Autorized = false;
                        tcs.SetResult(true);
                    }
                    else
                    {
                        tcs.SetResult(false);
                    }
                    connection.MessageRecived -= messageReceivedHandler;
                    LogoutExecuting = false;

                }

            };
            connection.MessageRecived += messageReceivedHandler;
            return await tcs.Task;
        }
    }
}
 