using TMServer.Data;
using TMServer.RequestsProcessing.SessionSystem.SessionManagers;

namespace TMServer
{
    internal static class GlobalProperties
    {
        public static Random random;
        public static Profiles profiles;
        public static ISessionManager sessionManager;

        static GlobalProperties()
        {
            random = new Random();
            profiles = new Profiles();
            sessionManager = new SessionManager();

        }
    }
}
