using Microsoft.EntityFrameworkCore;
using ClayAccessControl.Core.Entities;
namespace ClayAccessControl.Infrastructure.Data{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Door> Doors { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserDoorAccess> UserDoorAccesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserDoorAccess>()
                .HasKey(uda => new { uda.UserId, uda.DoorId });

        }
    }
}
