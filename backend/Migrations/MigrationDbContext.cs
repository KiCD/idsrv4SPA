using TB.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Migrations.Configuration;
using System;

namespace Migrations
{
    public class TokenServiceMigrationDbContextFactory : IDesignTimeDbContextFactory<MigrationDbContext>
    {
        MigrationDbContext IDesignTimeDbContextFactory<MigrationDbContext>.CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MigrationDbContext>();
            optionsBuilder.UseNpgsql(ConfigurationHelpers.GetConnectionString());

            return new MigrationDbContext(optionsBuilder.Options);
        }

    }

    internal static class ConfigurationHelpers
    {
        public static string GetConnectionString(int maxPoolSize = 2)
        {
            var projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { @"bin\" }, StringSplitOptions.None)[0];
            var configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            return GetConnectionString(configuration, maxPoolSize);

        }
        private static string GetConnectionString(IConfiguration configuration, int maxPoolSize = 500)
        {
            return NpgsqlHelper.BuildNpgsqlConnectionString(
                database: configuration.GetValue<string>(Config.DatabaseName),
                host: configuration.GetValue<string>(Config.DbHost),
                username: configuration.GetValue<string>(Config.DbUser),
                password: configuration.GetValue<string>(Config.DbPass),
            maxPoolSize: maxPoolSize);
        }
    }

    public class MigrationDbContext : ApplicationDbContext<MigrationDbContext>, IDataProtectionKeyContext
    {
        public MigrationDbContext(DbContextOptions<MigrationDbContext> options) : base(options)
        {
        }
    }
}
