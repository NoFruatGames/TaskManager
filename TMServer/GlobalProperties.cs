using TMServer.RequestsProcessing.SessionSystem.SessionManagers;

namespace TMServer
{
    internal static class GlobalProperties
    {
        public static readonly Random random = new Random();
        public static readonly ISessionManager sessionManager = new SessionManager();
    }
}
