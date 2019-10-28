using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TB.Entities
{
    public class ApplicationDbContext<TContext>: IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>> 
        where TContext : DbContext, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .ToTable("Users");

            builder.Entity<Role>()
                .ToTable("Roles");

            builder.Entity<UserRole>()
                .ToTable("UserRoles");

            builder.Entity<IdentityUserLogin<int>>()
                .ToTable("UserLogins");

            builder.Entity<IdentityUserClaim<int>>()
                .ToTable("UserClaims");

            builder.Entity<IdentityRoleClaim<int>>()
                .ToTable("RoleClaims");

            builder.Entity<IdentityUserToken<int>>()
                .ToTable("UserTokens");
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    }
    public class ApplicationDbContext : ApplicationDbContext<ApplicationDbContext>, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
