using TMServerLinker.ConnectionHandlers;
using TMServerLinker.Results;
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


        private bool MessageExecuting = false;



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
            if (InitSessionMessage.IsIdentity(obj))
            {
                InitSessionMessage? message = InitSessionMessage.TryParse(obj);
                if (message is not null && message.CheckTokenResult == InitSessionResult.Success)
                {
                    Autorized = true;
                }
                else
                {
                    Autorized = false;
                    sessionKey = string.Empty;
                    SaveSessionKeyToFile();
                }

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
            InitSessionMessage check = new InitSessionMessage()
            {
                IsRequest = true,
                SessionToken = string.IsNullOrEmpty(sessionKey) ? "none" : sessionKey
            };
            connection.AddMessageToQueue(check);
            MessageExecuting = false;
        }

        public void Dispose()
        {
            try
            {
                connection.Dispose();
            }
            catch { }

        }
        public async Task<RegisterResult> RegisterAccount(Profile profile)
        {
            if (MessageExecuting || Autorized) return RegisterResult.None;
            CreateAccountMessage request = new CreateAccountMessage() 
            {
                Username = profile.Username,
                Password = profile.Password,
                Email = profile.Email,
                IsRequest=true,
            };
            MessageExecuting = true;
            connection.AddMessageToQueue(request);
            var tcs = new TaskCompletionSource<RegisterResult>();
            Action<TMMessage>? messageReceivedHandler = null;
            messageReceivedHandler = (response) =>
            {
                if (CreateAccountMessage.IsIdentity(response))
                {
                    CreateAccountMessage? registerMessage = CreateAccountMessage.TryParse(response);
                    if (registerMessage is null) tcs.SetResult(RegisterResult.None);
                    else
                    {
                        if (registerMessage.CreateResult == CreateAccountResult.Success)
                            tcs.SetResult(RegisterResult.Success);
                        else if (registerMessage.CreateResult == CreateAccountResult.UsernameExist)
                            tcs.SetResult(RegisterResult.UsernameExist);
                    }
                    MessageExecuting = false;
                    connection.MessageRecived -= messageReceivedHandler;
                }
            };
            connection.MessageRecived += messageReceivedHandler;

            return await tcs.Task;
        }
        public async Task<Results.LoginResult> LoginToAccount(string username, string password)
        {
            if (MessageExecuting || Autorized) return Results.LoginResult.None;
            LoginMessage message = new LoginMessage()
            {
                IsRequest = true,
                Username = username,
                Password = password
            };
            connection.AddMessageToQueue(message);
            MessageExecuting = true;
            var tcs = new TaskCompletionSource<Results.LoginResult>();
            Action<TMMessage>? messageReceivedHandler = null;
            messageReceivedHandler = (response) =>
            {
                if (LoginMessage.IsIdentity(response))
                {
                    LoginMessage? loginMessage = LoginMessage.TryParse(response);
                    if (loginMessage is null) tcs.SetResult(Results.LoginResult.None);
                    else
                    {
                        if(loginMessage.LoginResult == TransferDataTypes.Results.LoginResult.Success)
                        {
                            sessionKey = loginMessage.SessionToken;
                            SaveSessionKeyToFile();
                            tcs.SetResult(Results.LoginResult.Success);
                        }
                        else
                        {
                            if (loginMessage.LoginResult == TransferDataTypes.Results.LoginResult.WrongUsername)
                                tcs.SetResult(Results.LoginResult.WrongLogin);
                            else if (loginMessage.LoginResult == TransferDataTypes.Results.LoginResult.WrongPassword)
                                tcs.SetResult(Results.LoginResult.WrongPassword);
                            else
                                tcs.SetResult(Results.LoginResult.None);
                        }
                    }
                    connection.MessageRecived -= messageReceivedHandler;
                    MessageExecuting = false;
                }
            };
            connection.MessageRecived += messageReceivedHandler;
            return await tcs.Task;
        }
        public async Task<bool> Logout()
        {
            if(MessageExecuting || !Autorized) return false;
            LogoutMessage message = new LogoutMessage()
            { 
                SessionToken =  string.IsNullOrEmpty(sessionKey)?"none":sessionKey,
                IsRequest = true,
            };
            connection.AddMessageToQueue(message);
            MessageExecuting = true;
            var tcs = new TaskCompletionSource<bool>();
            Action<TMMessage>? messageReceivedHandler = null;
            messageReceivedHandler = (response) =>
            {
                if (LogoutMessage.IsIdentity(response))
                {
                    sessionKey = string.Empty;
                    SaveSessionKeyToFile();
                    Autorized = false;
                    connection.MessageRecived -= messageReceivedHandler;
                    MessageExecuting = false;
                    tcs.SetResult(true);

                }

            };
            connection.MessageRecived += messageReceivedHandler;
            return await tcs.Task;
        }
    }
}
 