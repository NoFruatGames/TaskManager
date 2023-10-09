namespace TMServer.RequestsProcessing.SessionSystem
{
    internal class UserSession
    {
        public string Username { get; set; } = string.Empty;
        public string SessionToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
