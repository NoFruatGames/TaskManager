using TransferDataTypes.Attributes;
using TransferDataTypes.Results;

namespace TransferDataTypes.Messages
{
    public class CheckSessionMessage :TMMessage
    {
        [Newtonsoft.Json.JsonIgnore]
        [RequestProperty]
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
                if (value) setParameter("request", "check_session_token");
                else setParameter("response", "check_session_token");

            }
        }
        [Newtonsoft.Json.JsonIgnore]
        [ResponseProperty]
        public CheckSessionResult CheckTokenResult
        {
            get
            {
                if (!IsRequest) return getParameterValue<CheckSessionResult>("check_result");
                else return CheckSessionResult.None;

            }
            set
            {
                if (!IsRequest) setParameter("check_result", value);
                else setParameter("check_result", CheckSessionResult.None);
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
            public CheckSessionResult CheckTokenResult { get; init; } = CheckSessionResult.None;
            public bool IsRequest { get; init; }
            public bool CheckSucess { get; init; } = false;
        }
        private static CheckResult checkIdentity(TMMessage message)
        {
            CheckPropertyResult<string> sessionToken = getPropertyValue<string, LogoutMessage>("SessionToken", "session_token", message);
            CheckPropertyResult<CheckSessionResult> checkResult = getPropertyValue<CheckSessionResult, CheckSessionMessage>("CheckTokenResult", "check_result", message);
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
        public static CheckSessionMessage? TryParse(TMMessage message)
        {
            CheckResult check = checkIdentity(message);
            if (check.CheckSucess)
            {
                CheckSessionMessage m = new CheckSessionMessage()
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
