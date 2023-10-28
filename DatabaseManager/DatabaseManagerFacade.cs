using DatabaseManager.Tables;
using Microsoft.EntityFrameworkCore;
using DatabaseManager.Contexts;
using DatabaseManager.TableTools;

namespace DatabaseManager
{
    public class DatabaseManagerFacade :IDisposable
    {
        private ITmDbContext context;
        public UsersActions Users { get; }
        public SessionsActions Sessions { get; }
        public DatabaseManagerFacade(string connectionString)
        {
            context = new SQLServerContext(connectionString);
            Users = new UsersActions(context.Users);
            Sessions = new SessionsActions(context.Sessions);
        }
        public int SaveChanges()
        {
            return context.Save();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
