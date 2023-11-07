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
        public bool Autorized { get; private set; } = false;
        public bool IsConnected { get { return connection.IsConnected; } }

        private bool MessageExecuting = false;



        public TMClient(ConnectionHandler connection)
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
        }
        private void Connection_OnConnectionOpened()
        {
            
            
        }

        public void Dispose()
        {
            try
            {
                connection.NeedToReconnect = false;
                connection.Dispose();
            }
            catch { }

        }

        public async Task<Results.InitSessionResult> InitSession(string sessionKey)
        {
            if(MessageExecuting) return Results.InitSessionResult.None;
            if (Autorized) return Results.InitSessionResult.SessionNotClosed;
            if (!IsConnected) return Results.InitSessionResult.ServerIsNotAviliable;
            InitSessionMessage check = new InitSessionMessage()
            {
                IsRequest = true,
                SessionToken = string.IsNullOrEmpty(sessionKey) ? "none" : sessionKey
            };
            MessageExecuting = true;
            var tcs = new TaskCompletionSource<TMServerLinker.Results.InitSessionResult>();
            Action<TMMessage> messageReceivedHandler = null!;
            messageReceivedHandler = (response) =>
            {
                if(InitSessionMessage.IsIdentity(response))
                {
                    InitSessionMessage? initMessage = InitSessionMessage.TryParse(response);
                    if(initMessage is null) tcs.SetResult(Results.InitSessionResult.None);
                    else
                    {
                        if (initMessage.CheckTokenResult == TransferDataTypes.Results.InitSessionResult.Success)
                        {
                            tcs.SetResult(Results.InitSessionResult.Success);
                            Autorized = true;
                        }
                        else if (initMessage.CheckTokenResult == TransferDataTypes.Results.InitSessionResult.TokenNotExist)
                            tcs.SetResult(Results.InitSessionResult.TokenNotExist);
                        else if (initMessage.CheckTokenResult == TransferDataTypes.Results.InitSessionResult.TokenAlreadyUsing)
                            tcs.SetResult(Results.InitSessionResult.TokenAlreadyUsing);
                        else
                            tcs.SetResult(Results.InitSessionResult.None);
                    }
                    MessageExecuting = false;
                    connection.MessageRecived -= messageReceivedHandler;
                }
            };
            connection.MessageRecived += messageReceivedHandler;
            connection.AddMessageToQueue(check);
            return await tcs.Task;
        }


        public async Task<RegisterResult> RegisterAccount(Profile profile)
        {
            if (MessageExecuting || Autorized) return RegisterResult.None;
            if (!IsConnected) return RegisterResult.ServerNotAviliable;
            CreateAccountMessage request = new CreateAccountMessage() 
            {
                Username = profile.Username,
                Password = profile.Password,
                Email = profile.Email,
                IsRequest=true,
            };
            MessageExecuting = true;
            var tcs = new TaskCompletionSource<RegisterResult>();
            Action<TMMessage>? messageReceivedHandler = null;
            messageReceivedHandler = (response) =>
            {
                if (!IsConnected) { connection.MessageRecived -= messageReceivedHandler; tcs.TrySetCanceled(); };
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
            connection.AddMessageToQueue(request);
            return await tcs.Task;

        }
        public async Task<Results.LoginResult> LoginToAccount(string username, string password)
        {
            if (MessageExecuting) return new Results.LoginResult() { LoginStatus = LoginStatus.None, SessionToken = "" };
            if (Autorized) return new Results.LoginResult() { LoginStatus = LoginStatus.SessionNotClosed, SessionToken = "" };
            if (!IsConnected) return new Results.LoginResult() { LoginStatus = LoginStatus.ServerNotAviliable, SessionToken = "" };
            LoginMessage message = new LoginMessage()
            {
                IsRequest = true,
                Username = username,
                Password = password
            };
            MessageExecuting = true;
            var tcs = new TaskCompletionSource<Results.LoginResult>();
            Action<TMMessage>? messageReceivedHandler = null;
            messageReceivedHandler = (response) =>
            {
                if (LoginMessage.IsIdentity(response))
                {
                    LoginMessage? loginMessage = LoginMessage.TryParse(response);
                    if (loginMessage is null) tcs.SetResult(new Results.LoginResult() { LoginStatus = LoginStatus.None, SessionToken = "" });
                    else
                    {
                        if (loginMessage.LoginResult == TransferDataTypes.Results.LoginResult.Success)
                        {
                            Autorized = true;
                            tcs.SetResult(new Results.LoginResult() { LoginStatus = LoginStatus.Success, SessionToken = loginMessage.SessionToken });
                        }
                        else
                        {
                            if (loginMessage.LoginResult == TransferDataTypes.Results.LoginResult.WrongUsername)
                                tcs.SetResult(new Results.LoginResult() { SessionToken = "", LoginStatus = LoginStatus.WrongLogin });
                            else if (loginMessage.LoginResult == TransferDataTypes.Results.LoginResult.WrongPassword)
                                tcs.SetResult(new Results.LoginResult() { SessionToken = "", LoginStatus = LoginStatus.WrongPassword });
                            else if (loginMessage.LoginResult == TransferDataTypes.Results.LoginResult.CreateSessionServerError)
                                tcs.SetResult(new Results.LoginResult() { SessionToken = "", LoginStatus = LoginStatus.CreattingSessionServerError });
                            else
                                tcs.SetResult(new Results.LoginResult() { SessionToken = "", LoginStatus = LoginStatus.None });
                        }
                    }
                    connection.MessageRecived -= messageReceivedHandler;
                    MessageExecuting = false;
                }
            };
            connection.MessageRecived += messageReceivedHandler;
            connection.AddMessageToQueue(message);
            return await tcs.Task;

        }
        public async Task<ShutdownSessionResult> ShutdownSession(string sessionToken)
        {
            if(MessageExecuting) return ShutdownSessionResult.None;
            if (!Autorized) return ShutdownSessionResult.NotAuthorized;
            if (!IsConnected) return ShutdownSessionResult.ServerNotAviliable;
            LogoutMessage message = new LogoutMessage()
            { 
                SessionToken =  string.IsNullOrEmpty(sessionToken) ? "none": sessionToken,
                IsRequest = true,
            };
            MessageExecuting = true;
            var tcs = new TaskCompletionSource<ShutdownSessionResult>();
            Action<TMMessage>? messageReceivedHandler = null;
            messageReceivedHandler = (response) =>
            {
                if (LogoutMessage.IsIdentity(response))
                {
                    LogoutMessage? logoutMessage = LogoutMessage.TryParse(response);
                    if(logoutMessage is not null && logoutMessage.LogoutResult == LogoutResult.Success)
                    {
                        tcs.SetResult(ShutdownSessionResult.Success);
                    }
                    else
                    {
                        tcs.SetResult(ShutdownSessionResult.TokenNotExist);
                    }
                    Autorized = false;
                    MessageExecuting = false;
                    connection.MessageRecived -= messageReceivedHandler;
                }

            };
            connection.MessageRecived += messageReceivedHandler;
            connection.AddMessageToQueue(message);
            return await tcs.Task;
        }
    }
}
 