using DatabaseManager.Tables;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.TableTools
{
    public class SessionsActions : TableActions<Session>
    {
        internal SessionsActions(DbSet<Session> table) : base(table) { }
        public Session? GetRecordByToken(string sessionToken)
        {
            return table.FirstOrDefault(s => s.SessionToken == sessionToken);
        }
        public async Task<Session?> GetRecordByTokenAsync(string sessionToken)
        {
            return await table.FirstOrDefaultAsync(s => s.SessionToken == sessionToken);
        }

        public void RemoveSessionByToken(string sessionToken)
        {
            Session? sessionForRemove = GetRecordByToken(sessionToken);
            if (sessionForRemove is null) return;
            table.Remove(sessionForRemove);
        }
        public async Task RemoveSessionByTokenAsync(string sessionToken)
        {
            Session? sessionForRemove = await GetRecordByTokenAsync(sessionToken);
            if (sessionForRemove is null) return;
            table.Remove(sessionForRemove);
        }

        public List<Session> GetSessionsByUserId(int  userId)
        {
            return table.Where(s=> s.UserId == userId).ToList();

        }
        public List<Session> GetSessionsByUsername(string username)
        {
            return table.Where(s=> s.User.Username == username).ToList();   
        }
    }
}
