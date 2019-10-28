using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TB.TokenService.Configuration
{
    public static class PostgresConnectionConfiguration
    {
        public static int MaxAllowedTimeoutForEstablishingAConnection = 1024;
        public static int KeepAliveInSeconds = 30;
        public static int CommandTimeoutInSeconds = 300;
        public static int TimeoutForEstablishingAConnectionInSeconds = MaxAllowedTimeoutForEstablishingAConnection;
        public static int ConnectionIdleLifetime = 300;
        public static int ConnectionIdlePruningIntervalInSeconds = 300;
    }
}
