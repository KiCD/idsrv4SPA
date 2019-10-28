using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Migrations
{
    public class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Usage: " +
                                  "\n-update" +
                                  "\n-migrateTo migrationName" +
                                  "\n-seedData" +
                                  "\n-exit");
                Console.WriteLine("Enter input:");
                var line = Console.ReadLine();
                if (line == "-exit")
                {
                    break;
                }

                try
                {
                    HandleMigrationCommands(new[] { line }).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occured: " + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }

        }
        private static async Task HandleMigrationCommands(string[] args)
        {
            BaseDatabaseMigrator commonDbMigrator = new IdentityDatabaseMigrator();
            var command = args[0];
            switch (command)
            {
                case "-update":
                    {
                        await commonDbMigrator.UpdateDatabase();
                        break;
                    }

                case "-migrateTo":
                    {
                        await commonDbMigrator.MigrateDatabase(args);
                        break;
                    }

                case "-seedData":
                    {
                        await commonDbMigrator.SeedDatabase();
                        break;
                    }
            }
        }
    }
}
