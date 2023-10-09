using TransferDataTypes.Results;
namespace TransferDataTypes.Messages
{
    public class CreateAccountMessage : TMMessage
    {
        [Newtonsoft.Json.JsonIgnore]
        public string Username
        {
            get { return getParameterValue<string>("username") ?? string.Empty; }
            set { setParameter("username", value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        public string Password
        {
            get { return getParameterValue<string>("password") ?? string.Empty; }
            set { setParameter("password", value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        public string Email
        {
            get { return getParameterValue<string>("email") ?? string.Empty; }
            set { setParameter("email", value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        public  string SessionToken
        {
            get { return getParameterValue<string>("session_token") ?? string.Empty; }
            set { setParameter("session_token", value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        public required bool IsRequest
        {
            get
            {
                return !string.IsNullOrEmpty(getParameterValue<string>("request"));
            }
            set
            {
                if (value) setParameter("request", "create_account");
                else setParameter("response", "create_account");

            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public CreateAccountResult CreateResult
        {
            get 
            {
                if (!IsRequest) return getParameterValue<CreateAccountResult>("create_result");
                else return CreateAccountResult.None;

            }
            set
            {
                if (!IsRequest) setParameter("create_result", value);
                else setParameter("create_result", CreateAccountResult.None);
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
            public string Email { get; init; } = string.Empty;
            public string SessionToken { get; init; } = string.Empty;
            public bool IsRequest { get; init; }
            public CreateAccountResult CreateResult { get; init; } = CreateAccountResult.None;
            public bool CheckSucess { get; init; } = false;
        }
        private static CheckResult checkIdentity(TMMessage message)
        {
            string? username = message.getParameterValue<string>("username");
            string? password = message.getParameterValue<string>("password");
            string? email = message.getParameterValue<string>("email");
            string? sessionToken = message.getParameterValue<string>("session_token");
            CreateAccountResult createResult = message.getParameterValue<CreateAccountResult>("create_result");
            bool? isrequest = null;
            string? c = message.getParameterValue<string>("request");
            if (c is not null && c == "create_account") isrequest = true;
            c = message.getParameterValue<string>("response");
            if (c is not null && c == "create_account") isrequest = false;
            bool resultCheck = false;
            if(isrequest == true)
            {
                resultCheck = true;
            }
            else
            {
                if(createResult == CreateAccountResult.None)resultCheck= false;
                else resultCheck = true;
            }
            return new CheckResult()
            {
                Username = username ?? string.Empty,
                Password = password ?? string.Empty,
                Email = email ?? string.Empty,
                SessionToken = sessionToken ?? string.Empty,
                IsRequest = isrequest ?? false,
                CreateResult = createResult,
                CheckSucess = username is not null && password is not null && email is not null && sessionToken is not null && isrequest is not null && resultCheck
            };
        }
        public static CreateAccountMessage? TryParse(TMMessage message)
        {
            CheckResult check = checkIdentity(message);
            if (check.CheckSucess)
            {
                CreateAccountMessage m = new CreateAccountMessage()
                {
                    Email = check.Email,
                    Password = check.Password,
                    Username = check.Username,
                    IsRequest = check.IsRequest,
                    SessionToken = check.SessionToken,
                    CreateResult = check.CreateResult
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
