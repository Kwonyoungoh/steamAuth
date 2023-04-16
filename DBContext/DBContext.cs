using Microsoft.EntityFrameworkCore;
using steamAuth.Models;

namespace steamAuth.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
        public DbSet<UserInfoTable> UserInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfoTable>()
                .HasIndex(u => u._steamId)
                .IsUnique();
        }
    }
}
