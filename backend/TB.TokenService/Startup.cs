using System;
using TB.Entities;
using TB.TokenService.Configuration;
using TB.TokenService.Identity;
using TB.TokenService.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Serilog.Events;
using Serilog;
using Common.Infrastructure.Logging;
using Common.Infrastructure.Configuration;
using TB.TokenService.Services;
using IdentityServer4.AspNetIdentity;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TB.RestAPI
{
    public class Startup
    {
        protected LoggingLevels LoggingLevels { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LoggingLevels = configuration.GetLoggingLevels();
            Log.Logger = SerilogConfiguration.CreateSerilogLogger("TokenService",LoggingLevels);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            IdentityModelEventSource.ShowPII = true;
            var dbConnectionPoolSize = Configuration.GetValue<int>(Config.DbConnectionPoolSize);
            var connectionString = 
                Configuration.GetNpgsqlConnectionString(dbConnectionPoolSize != 0 ? 
                dbConnectionPoolSize : 
                PoolConfiguration.ConnectionPoolSize);
            ConfigureEntityFramework(services, connectionString);
            ConfigureAuthenticationRelatedServices(services);
            ConfigureIdentity(services);
            ConfigureAuthorization(services);
            ConfigureIdentityServer(services, connectionString, Configuration);
            ConfigureEmailing(services, Configuration);
            ConfigureHealthCheck(services);
            AddCors(services);

        }

        private void AddCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                AddCorsPolicy(options);
            });
        }

        private void AddCorsPolicy(Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions options)
        {
            options.AddPolicy("default", policy =>
            {
                policy.WithOrigins(
                        Configuration.GetValue<string>(Config.JsClientUrl))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("Content-Disposition");
            });

        }

        private void ConfigureHealthCheck(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();
        }

        private void ConfigureEmailing(IServiceCollection services, IConfiguration configuration)
        {
            
        }

        private void ConfigureIdentityServer(IServiceCollection services, string connectionString, IConfiguration configuration)
        {
            services.AddIdentityServer()
                  .AddInMemoryIdentityResources(IdentityServerConfiguration.GetIdentityResources())
                  .AddInMemoryApiResources(IdentityServerConfiguration.GetApiResources())
                  .AddInMemoryClients(IdentityServerConfiguration.GetClients(BuildClientUriConfigurationDictionary(Configuration)))
                  .AddAspNetIdentity<User>()
                  .AddDeveloperSigningCredential();
        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(AddPolicies);
        }
        private void AddPolicies(AuthorizationOptions options)
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("Bearer")
                .Build();
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            services.Configure<SecurityStampValidatorOptions>(options =>
                   options.ValidationInterval = TimeSpan.FromMinutes(AuthenticationConfiguration.SecurityStampValidationIntervalInMinutes));
            services.ConfigureApplicationCookie(options =>
            {
                //options.LoginPath = $"/Account/Login";
                //options.LogoutPath = $"/Account/Logout";
                //options.AccessDeniedPath = $"/Account/AccessDenied";
                options.SlidingExpiration = AuthenticationConfiguration.CookieSlidingExpiration;
                options.ExpireTimeSpan = TimeSpan.FromDays(AuthenticationConfiguration.PeristentCookieLifetimeInDays);
            });

            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.TokenLifespan = TimeSpan.FromHours(EmailTokensConfiguration.ResetPasswordTokenLifetimeInHours);
            });
            services.AddIdentity<User, Role>(config =>
            {
                config.SignIn.RequireConfirmedEmail = false;
                config.Password.RequireNonAlphanumeric = true;
                config.Password.RequireLowercase = true;
                config.Password.RequireUppercase = true;
                config.Password.RequireDigit = true;
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(24);
                config.Lockout.MaxFailedAccessAttempts = 5;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        private void ConfigureAuthenticationRelatedServices(IServiceCollection services)
        {
            services.AddTransient<ISignInManagerService, SignInManagerService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }

        private void ConfigureEntityFramework(IServiceCollection services, string connectionString)
        {
            services.AddEntityFrameworkNpgsql()
                 .AddDbContextPool<ApplicationDbContext>(options =>
                 {
                     options.UseNpgsql(connectionString, PgSqlAccessConfiguration.ConfigureTransientRetryPolicy());
                     options.ConfigureWarnings(warnings =>
                     {
                         if (LoggingLevels.Database > LogEventLevel.Information)
                             warnings.Ignore(CoreEventId.IncludeIgnoredWarning);
                     });
                     if (LoggingLevels.Database <= LogEventLevel.Information)
                         options.EnableDetailedErrors();
                 }, PoolConfiguration.ConnectionPoolSize);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            UseCors(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/hc");
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            


        }
        private static void UseCors(IApplicationBuilder app)
        {
            app.UseExceptionHandler((builder) => HttpExceptionHandlingExtensions.StartupHandler());
            app.UseCors("default");
        }

        protected Dictionary<string, ClientUriConfiguration> BuildClientUriConfigurationDictionary(IConfiguration configuration)
        {
            var baseJsClientUrl = configuration.GetValue<string>("TB_JS_CLIENT_URL");
            return new Dictionary<string, ClientUriConfiguration>()
           {
               { "tbjsclient", new ClientUriConfiguration()
                   {
                       Base = baseJsClientUrl,
                       PostLogin = $"{baseJsClientUrl }/callback",
                       SilentRenew = $"{baseJsClientUrl}/silent-refresh",
                       PostLogout = baseJsClientUrl

                   }}
           };
        }
    }
}
