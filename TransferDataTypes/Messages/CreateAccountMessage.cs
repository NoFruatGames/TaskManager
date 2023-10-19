using TransferDataTypes.Attributes;
using TransferDataTypes.Results;
namespace TransferDataTypes.Messages
{
    public class CreateAccountMessage : TMMessage
    {
        [Newtonsoft.Json.JsonIgnore]
        [RequestProperty]
        public string Username
        {
            get { return this.getParameterValue<string>("username") ?? string.Empty; }
            set { this.setParameter("username", value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        [RequestProperty]
        public string Password
        {
            get { return this.getParameterValue<string>("password") ?? string.Empty; }
            set { this.setParameter("password", value); }
        }
        [RequestProperty]
        [Newtonsoft.Json.JsonIgnore]
        public string Email
        {
            get { return this.getParameterValue<string>("email") ?? string.Empty; }
            set { this.setParameter("email", value); }
        }
        [Newtonsoft.Json.JsonIgnore]
        public required override bool IsRequest
        {
            get
            {
                return !string.IsNullOrEmpty(this.getParameterValue<string>("request"));
            }
            set
            {
                if (value) this.setParameter("request", "create_account");
                else setParameter("response", "create_account");

            }
        }
        [Newtonsoft.Json.JsonIgnore]
        [ResponseProperty]
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
            public bool IsRequest { get; init; }
            public CreateAccountResult CreateResult { get; init; } = CreateAccountResult.None;
            public bool CheckSucess { get; init; } = false;
        }
        private static CheckResult checkIdentity(TMMessage message)
        {
            CheckPropertyResult<string> username = getPropertyValue<string, CreateAccountMessage>("Username", "username", message);
            CheckPropertyResult<string> password = getPropertyValue<string, CreateAccountMessage>("Password", "password", message);
            CheckPropertyResult<string> email = getPropertyValue<string, CreateAccountMessage>("Email", "email", message);
            CheckPropertyResult<CreateAccountResult> createResult = getPropertyValue<CreateAccountResult, CreateAccountMessage>("CreateResult", "create_result", message);
            bool? isrequest = null;
            string? c = message.getParameterValue<string>("request");
            if (c is not null && c == "create_account") isrequest = true;
            c = message.getParameterValue<string>("response");
            if (c is not null && c == "create_account") isrequest = false;
            return new CheckResult()
            {
                Username = username.Property ?? string.Empty,
                Password = password.Property ?? string.Empty,
                Email = email.Property ?? string.Empty,
                IsRequest = isrequest ?? false,
                CreateResult = createResult.Property,
                CheckSucess = username.Success && password.Success && email.Success && createResult.Success && isrequest is not null
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
