using TB.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Migrations.Data
{
    class SeedData
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly ILogger<SeedData> _logger;
        private readonly UserManager<User> _userManager;
        private static SeedData _seedData = null;

        private SeedData()
        {
            _serviceProvider = CreateServiceProvider();
            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<SeedData>();
            _userManager = _serviceProvider.GetService<UserManager<User>>();
        }

        public static SeedData Default
        {
            get
            {
                if (_seedData == null)
                {
                    _seedData = new SeedData();
                }
                return _seedData;
            }
        }

        public static SeedData CreateInstance()
        {
            return new SeedData();
        }

        public async Task Seed()
        {
            await CreateRoles();
            await CreateUsers();
        }

        private async Task CreateUsers()
        {
            string fileString = GetEmbeddedResource("Migrations.Data.seed_user.json");
            var users = JsonConvert.DeserializeObject<List<UserCreationInfo>>(fileString);

            foreach (UserCreationInfo userCreationInfo in users)
            {
                await CreateOrUpdateUser(userCreationInfo);
            }
        }

        private async Task<IdentityResult> CreateOrUpdateUser(UserCreationInfo userCreationInfo)
        {
            var existingUser = await _userManager.FindByNameAsync(userCreationInfo.UserName);
            if (existingUser == null)
            {
                return await CreateUser(userCreationInfo);
            }
            else
            {
                return await UpdateUser(userCreationInfo, existingUser);
            }
        }

        private async Task<IdentityResult> UpdateUser(UserCreationInfo userCreationInfo, User existingUser)
        {
            existingUser.FirstName = !string.IsNullOrWhiteSpace(userCreationInfo.FirstName) ? userCreationInfo.FirstName?.Trim(): existingUser.FirstName;
            existingUser.LastName = !string.IsNullOrWhiteSpace(userCreationInfo.LastName) ? userCreationInfo.LastName?.Trim() : existingUser.LastName;
            existingUser.Email = !string.IsNullOrWhiteSpace(userCreationInfo.Email) ? userCreationInfo.Email?.Trim() : existingUser.Email;
            return await _userManager.UpdateAsync(existingUser);
        }

        private async Task<IdentityResult> CreateUser(UserCreationInfo userCreationInfo)
        {
            return await _userManager.CreateAsync(
                new User()
                {
                    UserName = userCreationInfo.UserName,
                    Email = userCreationInfo.Email,
                    FirstName = userCreationInfo.FirstName,
                    LastName = userCreationInfo.LastName,
                    IsEnabled = true,
                    EmailConfirmed = true
                }
                , userCreationInfo.Password); ;
        }

        private async Task CreateRoles()
        {
            var roleManager = _serviceProvider.GetService<RoleManager<Role>>();

            foreach (string roleName in Enum.GetNames(typeof(Roles)))
            {
                if (!roleManager.RoleExistsAsync(roleName).Result)
                {
                    Role role = new Role()
                    {
                        Name = roleName,
                        NormalizedName = roleName
                    };

                    await roleManager.CreateAsync(role);
                }
            }
        }

        private ServiceProvider CreateServiceProvider()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(ConfigurationHelpers.GetConnectionString());
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
                //options.EnableDetailedErrors();
            }, 5);

            services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddLogging(l => l.AddConsole());


            return services.BuildServiceProvider();
        }
        private string GetEmbeddedResource(string resourceFullPath)
        {
            var assembly = this.GetType().Assembly;
            using (var stream = assembly.GetManifestResourceStream(resourceFullPath))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
