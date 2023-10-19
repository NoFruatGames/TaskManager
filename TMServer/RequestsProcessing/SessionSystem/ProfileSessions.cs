using TMServer.ConnectionTools.Connections;
using TMServer.Data;

namespace TMServer.RequestsProcessing.SessionSystem
{
    internal class ProfileSessions
    {
        private Dictionary<Profile, List<UserSession>> profilesSessions = new Dictionary<Profile, List<UserSession>>();
        public ProfileSessions()
        {
            GlobalProperties.profiles.ProfileAdded += Profiles_ProfileAdded;
            GlobalProperties.profiles.ProfileDeleated += Profiles_ProfileDeleated;
        }

        private void Profiles_ProfileDeleated(Profile obj)
        {
            profilesSessions.Remove(obj);
        }

        private void Profiles_ProfileAdded(Profile obj)
        {
            profilesSessions.Add(obj, new List<UserSession>());
        }

        public void AddSessionToProfile(Profile profile, UserSession session)
        {
            if (profilesSessions.ContainsKey(profile))
                profilesSessions[profile].Add(session);
        }
        public List<UserSession>? GetSessionsByProfile(Profile profile)
        {
            if (!profilesSessions.ContainsKey(profile)) return null;
            return profilesSessions[profile];
        }
        public Profile? GetProfileBySession(UserSession session)
        {
            foreach (var profileSessions in profilesSessions)
            {
                if (profileSessions.Value.Contains(session))
                {
                    return profileSessions.Key;
                }
            }

            return null;
        }

        public UserSession? GetSessionByToken(string sessionToken)
        {
            foreach(var session in profilesSessions)
            {
                foreach(var s in session.Value)
                {
                    if(s.SessionToken == sessionToken) return s;
                }
            }
            return null;
        }
        //public void RemoveSessionFromProfile(Profile profile, UserSession session)
        //{
        //    if (!profilesSessions.ContainsKey(profile)) return;
        //    profilesSessions[profile].Remove(session);
        //}
        public void RemoveSession(UserSession session)
        {
            foreach(var profiles in profilesSessions)
            {
                if (profiles.Value.Remove(session))
                    return;
            }
        }
    }
}
