﻿using TransferDataTypes.Attributes;
using TransferDataTypes.Results;

namespace TransferDataTypes.Messages
{
    public class LogoutMessage : TMMessage
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
                if (value) setParameter("request", "logout");
                else setParameter("response", "logout");

            }
        }
        [Newtonsoft.Json.JsonIgnore]
        [ResponseProperty]
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
            CheckPropertyResult<string> sessionToken = getPropertyValue<string, LogoutMessage>("SessionToken", "session_token", message);
            CheckPropertyResult<LogoutResult> logoutResult = getPropertyValue<LogoutResult, LogoutMessage>("LogoutResult", "logout_result", message);
            bool? isrequest = null;
            string? c = message.getParameterValue<string>("request");
            if (c is not null && c == "logout") isrequest = true;
            c = message.getParameterValue<string>("response");
            if (c is not null && c == "logout") isrequest = false;
            return new CheckResult()
            {
                SessionToken = sessionToken.Property??string.Empty,
                IsRequest = isrequest ?? false,
                LogoutResult = logoutResult.Property,
                CheckSucess = sessionToken.Success && logoutResult.Success && isrequest is not null
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
