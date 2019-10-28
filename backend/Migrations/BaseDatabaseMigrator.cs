using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Migrations
{
    public abstract class BaseDatabaseMigrator
    {
        public abstract Task SeedDatabase(params string[] args);

        public abstract Task UpdateDatabase();

        public abstract Task MigrateDatabase(string[] args);

        public virtual Task ClearData()
        {
            return Task.FromResult(1);
        }

        public virtual Task<bool> RunDemoScripts()
        {
            return Task.FromResult(true);
        }
    }
}
