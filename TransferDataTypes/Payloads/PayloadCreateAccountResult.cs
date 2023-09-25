namespace TransferDataTypes.Payloads
{
    public struct PayloadCreateAccountResult
    {
        public CreateAccountResult Result { get; set; } = CreateAccountResult.None;
        public string SessionKey { get; set; } = string.Empty;
        public PayloadCreateAccountResult() { }
    }
    public enum CreateAccountResult
    {
        None, Success, UsernameAlreadyExist, ServerError
    }
}
