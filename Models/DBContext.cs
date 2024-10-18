using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Models
{
    public class YourDbContext : DbContext
    {
        public YourDbContext(DbContextOptions<YourDbContext> options) : base(options)
        { 
        
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Approval> Approvals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optionally configure the entity properties to match the existing database
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Lecturer>().ToTable("Lecturer");
            modelBuilder.Entity<Claim>().ToTable("Claim");
            modelBuilder.Entity<Admin>().ToTable("Admin");
        }
    }
}
