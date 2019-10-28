using Common.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using System;

namespace Common.Infrastructure.Logging
{
    public static class LoggingConfigurationExtensions
    {
        public static LoggingLevels GetLoggingLevels(this IConfiguration configuration)
        {
            var loggingLevels = new LoggingLevels()
            {
                Database = GetLogLevel(configuration, CommonConfig.DatabaseLogLevel, LogEventLevel.Error),
                Authentication = GetLogLevel(configuration, CommonConfig.AuthenticationLogLevel, LogEventLevel.Information),
                General = GetLogLevel(configuration, CommonConfig.GeneralLogLevel, LogEventLevel.Information)
            };
            return loggingLevels;
        }

        private static LogEventLevel GetLogLevel(IConfiguration configuration, string configurationKeyName,
            LogEventLevel defaultValue)
        {
            var configurationValueAsString = configuration.GetValue<string>(configurationKeyName);
            if (string.IsNullOrWhiteSpace(configurationValueAsString)) return defaultValue;
            if (Enum.TryParse<LogEventLevel>(configurationValueAsString, true, out var value))
            {
                return (LogEventLevel)value;
            }

            return defaultValue;
        }
    }
}
