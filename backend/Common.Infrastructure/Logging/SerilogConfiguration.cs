using Serilog;
using Serilog.Exceptions;
using Common.Infrastructure.Configuration;

namespace Common.Infrastructure.Logging
{
    public static class SerilogConfiguration
    {
        public static ILogger CreateSerilogLogger(string serviceName,LoggingLevels loggingLevels)
        {
            LoggerConfiguration logConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(loggingLevels.General)
                .MinimumLevel.Override("Microsoft", loggingLevels.General)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", loggingLevels.Authentication)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication.OpenIdConnect", loggingLevels.Authentication)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", loggingLevels.Database)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.File($"/data/logging/{serviceName}.log", rollingInterval: RollingInterval.Day);

            return logConfiguration.CreateLogger();
        }
    }
}
