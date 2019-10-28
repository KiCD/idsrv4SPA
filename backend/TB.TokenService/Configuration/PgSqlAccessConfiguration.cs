using Microsoft.Extensions.Configuration;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TB.TokenService.Configuration
{
    public static class PgSqlAccessConfiguration
    {
        public static string GetNpgsqlConnectionString(this IConfiguration configuration, int maxPoolSize = 500)
        {
            return BuildNpgsqlConnectionString(
                database: configuration.GetValue<string>(Config.DatabaseName),
                host: configuration.GetValue<string>(Config.DbHost),
                username: configuration.GetValue<string>(Config.DbUser),
                password: configuration.GetValue<string>(Config.DbPass),
            maxPoolSize: maxPoolSize);
        }
        public static string BuildNpgsqlConnectionString(string database, string host, string username, string password, int port = 5432, bool pooling = true, int maxPoolSize = 500)
        {
            var npgsqlbuilder =
                new NpgsqlConnectionStringBuilder
                {
                    Database = database,
                    Host = host,
                    Username = username,
                    Password = password,
                    Pooling = true,
                    KeepAlive = PostgresConnectionConfiguration.KeepAliveInSeconds,
                    CommandTimeout = PostgresConnectionConfiguration.CommandTimeoutInSeconds,
                    TcpKeepAlive = true,
                    NoResetOnClose = false,
                    MaxPoolSize = maxPoolSize,
                    MinPoolSize = 0,
                    Timeout = PostgresConnectionConfiguration.TimeoutForEstablishingAConnectionInSeconds,
                    Enlist = false,
                    ConnectionIdleLifetime = PostgresConnectionConfiguration.ConnectionIdleLifetime,
                    ConnectionPruningInterval = PostgresConnectionConfiguration.ConnectionIdlePruningIntervalInSeconds,
                    Port = port
                };

            return npgsqlbuilder.ConnectionString;
        }

        public static Action<NpgsqlDbContextOptionsBuilder> ConfigureTransientRetryPolicy()
        {
            return builder => builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(1), new List<string>());
        }
    }
}
