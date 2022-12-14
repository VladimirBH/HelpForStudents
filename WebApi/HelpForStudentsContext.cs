using Microsoft.EntityFrameworkCore;
using WebApi.DataAccess.Database;
namespace WebApi
{
    public class HelpForStudentsContext : DbContext
    {
        public HelpForStudentsContext (DbContextOptions<HelpForStudentsContext> options)
            : base(options)    
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<RefreshSession> RefreshSessions { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime with time zone").HasDefaultValue(DateTimeOffset.Now);
                entity.Property(e => e.UpdateDate).HasColumnType("datetime with time zone");
            });
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime with time zone").HasDefaultValue(DateTimeOffset.Now);
                entity.Property(e => e.UpdateDate).HasColumnType("datetime with time zone");
            });
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime with time zone").HasDefaultValue(DateTimeOffset.Now);
            });
            modelBuilder.Entity<Theme>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime with time zone").HasDefaultValue(DateTimeOffset.Now);
                entity.Property(e => e.UpdateDate).HasColumnType("datetime with time zone");
            });
            modelBuilder.Entity<RefreshSession>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime with time zone").HasDefaultValue(DateTimeOffset.Now);
            });
        }
    }
}