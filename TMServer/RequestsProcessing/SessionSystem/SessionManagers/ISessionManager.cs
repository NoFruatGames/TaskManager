namespace TMServer.RequestsProcessing.SessionSystem.SessionManagers
{
    internal interface ISessionManager
    {
        UserSession CreateSession(string username);
        bool ValidateSession(string sessionToken);
        void ExpireSession(string sessionToken);
    }
}
