using DatabaseManager.Tables;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.Contexts
{
    internal interface ITmDbContext :IDisposable
    {
        DbSet<User> Users { get; set; }
        DbSet<Session> Sessions { get; set; }
        int Save();
        Task<int> SaveAsync();
    }
}
