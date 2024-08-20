using ASPNetCoreMVCSample.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASPNetCoreMVCSample.Data
{
    public class MyDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var hasher = new PasswordHasher<ApplicationUser>();

            // Create Admin role
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            // Create an Admin user
            builder.Entity<ApplicationUser>().HasData(new ApplicationUser
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                UserName = "EcommerceAdmin",
                NormalizedUserName = "ECOMMERCEADMIN",
                Email = "admin@ecommerce.com",
                NormalizedEmail = "ADMIN@ECOMMERCE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Pa$$w0rd"),
                SecurityStamp = string.Empty,
                FirstName = "AdminFirstName",
                LastName = "AdminLastName"
            });

            // Assign the Admin role to the created user
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210", // Admin Role
                UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9" // Admin User
            });
        }
    }
}
