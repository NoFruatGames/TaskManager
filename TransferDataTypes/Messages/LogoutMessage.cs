using TransferDataTypes.Results;

namespace TransferDataTypes.Messages
{
    public class LogoutMessage : TMMessage
    {
        [Newtonsoft.Json.JsonIgnore]
        public string SessionToken
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
                if (value) setParameter("request", "logout");
                else setParameter("response", "logout");

            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public LogoutResult LogoutResult
        {
            get
            {
                if (!IsRequest) return getParameterValue<LogoutResult>("logout_result");
                else return LogoutResult.None;

            }
            set
            {
                if (!IsRequest) setParameter("logout_result", value);
                else setParameter("logout_result", LogoutResult.None);
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
            public LogoutResult LogoutResult { get; init; } = LogoutResult.None;
            public bool IsRequest { get; init; }
            public bool CheckSucess { get; init; } = false;
        }
        private static CheckResult checkIdentity(TMMessage message)
        {
            string? sessionToken = message.getParameterValue<string>("session_token");
            LogoutResult logoutResult = message.getParameterValue<LogoutResult>("logout_result");
            bool? isrequest = null;
            string? c = message.getParameterValue<string>("request");
            if (c is not null && c == "logout") isrequest = true;
            c = message.getParameterValue<string>("response");
            if (c is not null && c == "logout") isrequest = false;
            bool resultCheck = false;
            if (isrequest == true)
            {
                resultCheck = true;
            }
            else
            {
                if (logoutResult == LogoutResult.None) resultCheck = false;
                else resultCheck = true;
            }
            return new CheckResult()
            {
                SessionToken = sessionToken ?? string.Empty,
                IsRequest = isrequest ?? false,
                LogoutResult = logoutResult,
                CheckSucess = sessionToken is not null && isrequest is not null && resultCheck
            };
        }
        public static LogoutMessage? TryParse(TMMessage message)
        {
            CheckResult check = checkIdentity(message);
            if (check.CheckSucess)
            {
                LogoutMessage m = new LogoutMessage()
                {
                    IsRequest = check.IsRequest,
                    SessionToken = check.SessionToken,
                    LogoutResult = check.LogoutResult
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
