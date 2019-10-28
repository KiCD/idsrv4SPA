using Migrations.Configuration;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Migrations
{
    /// <summary>
    /// helper to configure Npgsql settings
    /// </summary>
    public class NpgsqlHelper
    {
        /// <summary>
        /// Build a standard Npgsql connection string.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="host"></param>
        /// <param name="username"></param>
        /// <param name="password"></paclcram>
        /// <param name="pooling"></param>
        /// <param name="maxPoolSize"></param>
        /// <returns></returns>
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
