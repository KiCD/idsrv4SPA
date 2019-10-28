using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Migrations.Data;
using System;
using System.Threading.Tasks;

namespace Migrations
{
    public class IdentityDatabaseMigrator : BaseDatabaseMigrator
    {
        public override async Task MigrateDatabase(string[] args)
        {
            using (MigrationDbContext context = new MigrationDbContext(GetOptions()))
            {
                Console.WriteLine($"Starting database migration to... {args[0]}");
                await context.GetService<IMigrator>().MigrateAsync(args[0]);
                Console.WriteLine("Update completed.");
            }
        }

        public override async Task SeedDatabase(params string[] args)
        {
            Console.WriteLine("Starting database seed...");
            await SeedData.Default.Seed();
            Console.WriteLine("Update completed.");
        }

        public override async Task UpdateDatabase()
        {
            var connectionString = ConfigurationHelpers.GetConnectionString();

            using (MigrationDbContext context = new MigrationDbContext(GetOptions()))
            {
                Console.WriteLine("Starting database update...");
                await context.Database.MigrateAsync();
                Console.WriteLine("Update completed.");
            }
        }
        private DbContextOptions<MigrationDbContext> GetOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MigrationDbContext>();
            optionsBuilder.UseNpgsql(ConfigurationHelpers.GetConnectionString());

            return optionsBuilder.Options;
        }
    }
}
