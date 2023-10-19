using TMServer.ConnectionTools.Connections;
using TMServer.Data;
using TransferDataTypes;

namespace TMServer.RequestsProcessing.SessionSystem.SessionManagers
{
    internal class SessionManager :ISessionManager
    {
        public int TokenLenght { get { return UserSession.TokenLenght; } }
        private ProfileSessions profileSessions = new ProfileSessions();

        public UserSession? CreateSession(string username)
        {
            Profile? profile = GlobalProperties.profiles[username];
            if (profile is null) return null;
            UserSession session = new UserSession(null);
            profileSessions.AddSessionToProfile(profile, session);
            return session;
        }

        public bool ValidateSession(string sessionToken)
        {
            UserSession? session = this[sessionToken];
            if(session is null) return false;
            Profile? profile = profileSessions.GetProfileBySession(session);
            if(profile is null) return false;
            return true;
        }

        public void ExpireSession(string sessionToken)
        {
            UserSession? session = this[sessionToken];
            if(session is null) return;
            profileSessions.RemoveSession(session);
        }
        public async Task NotifyAllConnections(TMMessage message, Profile profile)
        {
            var sessions = profileSessions.GetSessionsByProfile(profile);
            if(sessions is null) return;
            foreach(var session in sessions)
            {
                if (session.Connection is null) continue;
                await session.Connection.SendMessageAsync(message);

            }
        }
        public void DeleateAllSessionsFromProfile(Profile profile)
        {
            var sessions = profileSessions.GetSessionsByProfile(profile);
            if (sessions is null) return;
            foreach(var session in sessions)
            {
                profileSessions.RemoveSession(session);
            }
        }

        public UserSession? this[string sessionToken]
        {
            get
            {
                return profileSessions.GetSessionByToken(sessionToken);
            }
        }
    }
}
