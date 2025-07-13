using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Data.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "4c75fd3c-2163-4d0c-a753-52dafb027b01",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole
                {
                    Id = "5472eb52-7931-4a41-85f0-24c03c7f0b88",
                    Name = "Client",
                    NormalizedName = "CLIENT",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );
        }

    }
}
