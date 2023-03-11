using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webapiPOC.Model;

namespace webapiPOC.Data
{
    public class apiContextClass:IdentityDbContext<IdentityUser>
    {
        public apiContextClass(DbContextOptions options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
        }

        //created method for seeding roles of perticular model
        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
            new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
            new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" },
            new IdentityRole() { Name = "HR", ConcurrencyStamp = "3", NormalizedName = "HR" }
            );

        }

        public DbSet<signup> signups { get; set; }
    }
}
