using DatabaseManager.Tables;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.TableTools
{
    public class UsersActions : TableActions<User>
    {
        internal UsersActions(DbSet<User> table) : base(table)
        {

        }

        public User? GetRecordByUsername(string username)
        {
            return table.FirstOrDefault(t => t.Username == username);
        } 
        public async Task<User?> GetRecordByUsernameAsync(string username)
        {
            return await table.FirstOrDefaultAsync(t=>t.Username == username);
        }
        public void RemoveUserByUsername(string username)
        {
            User? userForRemove = GetRecordByUsername(username);
            if (userForRemove is null) return;
            table.Remove(userForRemove);
        }
        public async Task RemoveUserByUsernameAsync(string username)
        {
            User? userForRemove = await GetRecordByUsernameAsync(username);
            if (userForRemove is null) return;
            table.Remove(userForRemove);
        }
    }
}
