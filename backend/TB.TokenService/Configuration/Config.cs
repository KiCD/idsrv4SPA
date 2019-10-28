using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TB.TokenService.Configuration
{
    public static class Config
    {
        public const string DatabaseName = "TB_TKS_DBNAME";
        public const string DbHost = "TB_TKS_DBHOST";
        public const string DbUser = "TB_TKS_DBUSER";
        public const string DbPass = "TB_TKS_DBPASSWORD";
        public const string DbPort = "TB_TKS_PORT";

        public const string DbConnectionPoolSize = "TB_DB_CONNECTION_POOL_SIZE";
        public const string DbContextPoolSize = "TB_DB_CONTEXT_POOL_SIZE";

        public static string JsClientUrl = "TB_JS_CLIENT_URL";
    }
}
