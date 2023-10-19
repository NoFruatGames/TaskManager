using TransferDataTypes.Attributes;
using TransferDataTypes.Results;

namespace TransferDataTypes.Messages
{
    public class InitSessionMessage :TMMessage
    {
        [Newtonsoft.Json.JsonIgnore]
        [RequestProperty]
        public string SessionToken
        {
            get { return this.getParameterValue<string>("session_token") ?? string.Empty; }
            set { this.setParameter("session_token", value); }
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
                if (value) this.setParameter("request", "check_session_token");
                else this.setParameter("response", "check_session_token");

            }
        }
        [Newtonsoft.Json.JsonIgnore]
        [ResponseProperty]
        public InitSessionResult CheckTokenResult
        {
            get
            {
                if (!IsRequest) return this.getParameterValue<InitSessionResult>("check_result");
                else return InitSessionResult.None;

            }
            set
            {
                if (!IsRequest) this.setParameter("check_result", value);
                else this.setParameter("check_result", InitSessionResult.None);
            }
        }
        public static new LogoutMessage? Deserialize(string text)
        {
            return DeserializeFromObject<LogoutMessage>(text);
        }

        private struct CheckResult
        {
            public CheckResult() { }
            public string SessionToken { get; init; } = string.Empty;
            public InitSessionResult CheckTokenResult { get; init; } = InitSessionResult.None;
            public bool IsRequest { get; init; }
            public bool CheckSucess { get; init; } = false;
        }
        private static CheckResult checkIdentity(TMMessage message)
        {
            CheckPropertyResult<string> sessionToken = getPropertyValue<string, LogoutMessage>("SessionToken", "session_token", message);
            CheckPropertyResult<InitSessionResult> checkResult = getPropertyValue<InitSessionResult, InitSessionMessage>("CheckTokenResult", "check_result", message);
            bool? isrequest = null;
            string? c = message.getParameterValue<string>("request");
            if (c is not null && c == "check_session_token") isrequest = true;
            c = message.getParameterValue<string>("response");
            if (c is not null && c == "check_session_token") isrequest = false;
            return new CheckResult()
            {
                SessionToken = sessionToken.Property ?? string.Empty,
                IsRequest = isrequest ?? false,
                CheckTokenResult = checkResult.Property,
                CheckSucess = sessionToken.Success && checkResult.Success && isrequest is not null
            };
        }
        public static InitSessionMessage? TryParse(TMMessage message)
        {
            CheckResult check = checkIdentity(message);
            if (check.CheckSucess)
            {
                InitSessionMessage m = new InitSessionMessage()
                {
                    IsRequest = check.IsRequest,
                    SessionToken = check.SessionToken,
                    CheckTokenResult = check.CheckTokenResult
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
