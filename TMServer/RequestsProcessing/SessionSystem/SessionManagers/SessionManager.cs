namespace TMServer.RequestsProcessing.SessionSystem.SessionManagers
{
    internal class SessionManager : ISessionManager
    {
        private List<UserSession> sessions = new List<UserSession>();

        public UserSession CreateSession(string username)
        {
            UserSession session = new UserSession()
            {
                CreatedAt = DateTime.Now,
                Username = username,
                SessionToken = GenerateSessionToken()
            };
            sessions.Add(session);
            return session;
        }
        private string GenerateSessionToken()
        {
            string token = string.Empty;
            for(int i = 0; i < 12; ++i)
            {
                token += GlobalProperties.random.Next(1, 9).ToString();
            }
            return string.Empty;
        }

        public bool ValidateSession(string sessionToken)
        {
            foreach (var s in sessions)
            {
                if (s.SessionToken == sessionToken)
                    return true;
            }
            return false;
        }

        public void ExpireSession(string sessionToken)
        {
            for(int i = 0; i < sessions.Count; ++i)
            {
                if (sessions[i].SessionToken == sessionToken)
                {
                    sessions.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
