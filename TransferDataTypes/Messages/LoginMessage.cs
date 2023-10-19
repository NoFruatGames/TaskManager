using TransferDataTypes.Attributes;
using TransferDataTypes.Results;

namespace TransferDataTypes.Messages
{
    public class LoginMessage : TMMessage
    {
        [Newtonsoft.Json.JsonIgnore]
        [RequestProperty]
        public string Username
        {
            get { return getParameterValue<string>("username") ?? string.Empty; }
            set { setParameter("username", value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        [RequestProperty]
        public string Password
        {
            get { return getParameterValue<string>("password") ?? string.Empty; }
            set { setParameter("password", value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        [ResponseProperty]
        public string SessionToken
        {
            get { return getParameterValue<string>("session_token") ?? string.Empty; }
            set { setParameter("session_token", value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        public required override bool IsRequest
        {
            get
            {
                return !string.IsNullOrEmpty(getParameterValue<string>("request"));
            }
            set
            {
                if (value) setParameter("request", "login");
                else setParameter("response", "login");

            }
        }
        [Newtonsoft.Json.JsonIgnore]
        [ResponseProperty]
        public LoginResult LoginResult
        {
            get
            {
                if (!IsRequest) return this.getParameterValue<LoginResult>("login_result");
                else return LoginResult.None;

            }
            set
            {
                if (!IsRequest) this.setParameter("login_result", value);
                else this.setParameter("login_result", LoginResult.None);
            }
        }
        public static new CreateAccountMessage? Deserialize(string text)
        {
            return DeserializeFromObject<CreateAccountMessage>(text);
        }

        private struct CheckResult
        {
            public CheckResult() { }
            public string Username { get; init; } = string.Empty;
            public string Password { get; init; } = string.Empty;
            public string SessionToken { get; init; } = string.Empty;
            public bool IsRequest { get; init; }
            public LoginResult LoginResult { get; init; } = LoginResult.None;
            public bool CheckSucess { get; init; } = false;
        }
        private static CheckResult checkIdentity(TMMessage message)
        {
            CheckPropertyResult<string> username = getPropertyValue<string, LoginMessage>("Username", "username", message);
            CheckPropertyResult<string> password = getPropertyValue<string, LoginMessage>("Password", "password", message);
            CheckPropertyResult<string> sessionToken = getPropertyValue<string, LoginMessage>("SessionToken", "session_token", message);
            CheckPropertyResult<LoginResult> loginResult = getPropertyValue<LoginResult, LoginMessage>("LoginResult", "login_result", message);
            bool? isrequest = null;
            string? c = message.getParameterValue<string>("request");
            if (c is not null && c == "login") isrequest = true;
            c = message.getParameterValue<string>("response");
            if (c is not null && c == "login") isrequest = false;
            return new CheckResult()
            {
                Username = username.Property ?? string.Empty,
                Password = password.Property ?? string.Empty,
                SessionToken = sessionToken.Property ?? string.Empty,
                IsRequest = isrequest ?? false,
                LoginResult = loginResult.Property,
                CheckSucess = username.Success && password.Success && sessionToken.Success && loginResult.Success && isrequest is not null
            };
        }
        public static LoginMessage? TryParse(TMMessage message)
        {
            CheckResult check = checkIdentity(message);
            if (check.CheckSucess)
            {
                LoginMessage m = new LoginMessage()
                {
                    Password = check.Password,
                    Username = check.Username,
                    SessionToken = check.SessionToken,
                    IsRequest = check.IsRequest,
                    LoginResult = check.LoginResult
                };
                return m;
            }
            return null;
        }
        public static bool IsIdentity(TMMessage message)
        {
            return checkIdentity(message).CheckSucess;
        }
    }
}
