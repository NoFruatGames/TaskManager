using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer.RequestProcessing
{
    internal class SessionManager
    {
        private List<Session> sessions = new List<Session>();
        Random random;
        public SessionManager(Random random)
        {
            this.random = random;
        }
        public string CreateSession()
        {
            Session s = new Session(random);
            sessions.Add(s);
            return s.Key;
        }
        public bool CreateSession(string key)
        {
            if (sessionExist(key) || string.IsNullOrEmpty(key)) return false;
            Session s = new Session(key);
            sessions.Add(s);
            return true;
        }
        public bool DeleteSessionByKey(string key)
        {
            for (int i = 0; i < sessions.Count; i++)
            {
                if (sessions[i].Key == key)
                {
                    sessions.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public bool RegisterClientForSession(string clientId, string sessionKey)
        {
            for (int i = 0; i < sessions.Count; i++)
            {
                if (sessions[i].Key == sessionKey)
                {
                    sessions[i].ClientIds.Add(clientId);
                    return true;
                }
            }
            return false;

        }
        public List<string>? GetClientIdsBySession(string sessionKey)
        {
            for (int i = 0; i < sessionKey.Length; i++)
            {
                if (sessions[i].Key == sessionKey)
                {
                    return sessions[i].ClientIds;
                }
            }
            return null;
        }
        public bool RemoveClientFromSession(string clientId)
        {
            for (int i = 0; i < sessions.Count; i++)
            {
                if (sessions[i].ClientIds.Remove(clientId))
                    return true;
            }
            return false;
        }
        private bool sessionExist(string sessionKey)
        {
            for (int i = 0; i < sessions.Count; i++)
            {
                if (sessions[i].Key == sessionKey)
                    return true;
            }
            return false;
        }

    }
}
