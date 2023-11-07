namespace TMServerLinker.Results
{
    public enum LoginStatus
    {
        None, ServerNotAviliable, Success, WrongLogin, WrongPassword, SessionNotClosed, CreattingSessionServerError
    }
    public struct LoginResult
    {
        public LoginStatus LoginStatus { get; init; } = LoginStatus.None;
        public string SessionToken { get; init; } = string.Empty;
        public LoginResult() { }
    }
}
