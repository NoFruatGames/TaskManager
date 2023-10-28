using DatabaseManager.Tables;
using Microsoft.EntityFrameworkCore;

namespace DatabaseManager.Contexts
{
    internal class SQLServerContext : DbContext, ITmDbContext
    {

        private string connectionString = null!;
        public SQLServerContext(string connectionString)
        {
            this.connectionString = connectionString;
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasColumnType("varchar(20)")
                .IsRequired();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .HasColumnType("varchar(30)")
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasColumnType("varchar(30)")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.Gender)
                .HasColumnType("varchar(1)")
                .IsRequired(false);



            modelBuilder.Entity<Session>()
                .Property(u => u.CreatedAt)
                .HasColumnType("datetime");

            modelBuilder.Entity<Session>()
                .Property(u => u.SessionToken)
                .HasColumnType("varchar(12)")
                .IsRequired();

            modelBuilder.Entity<Session>()
                .Property(u => u.UserId)
                .HasColumnType("int")
                .IsRequired();

            modelBuilder.Entity<Session>()
                .HasIndex(u => u.SessionToken)
                .IsUnique();

            modelBuilder.Entity<User>()
                    .HasMany(s => s.Sessions)
                    .WithOne(u => u.User)
                    .HasForeignKey(u => u.UserId)
                    .HasPrincipalKey(u => u.Id)
                    .OnDelete(DeleteBehavior.Cascade);

        }

        public int Save()
        {
            return SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await SaveChangesAsync();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }

    }
}
