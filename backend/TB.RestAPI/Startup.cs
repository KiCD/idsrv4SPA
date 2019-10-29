using Common.Infrastructure;
using Common.Infrastructure.Configuration;
using Common.Infrastructure.Logging;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using TB.RestAPI.Configuration;
using TB.TokenService.Infrastructure;

namespace TB.RestAPI
{
    public class Startup
    {
        protected LoggingLevels LoggingLevels { get; private set; }
        private readonly IWebHostEnvironment _environment;
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            LoggingLevels = configuration.GetLoggingLevels();
            _environment = environment;
            Log.Logger = SerilogConfiguration.CreateSerilogLogger("RestApi",LoggingLevels);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            ConfigureAuthorization(services);
            AddCors(services);
            services.AddControllers();
        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // base-address of your identityserver
                    options.Authority = Configuration.GetValue<string>(Config.TokenServiceInternalUrl);

                    // name of the API resource
                    options.Audience = CommonConfig.Api_Audience;
                    options.RequireHttpsMetadata = !_environment.IsDevelopment();
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidIssuers = new List<string>() { Configuration.GetValue<string>(Config.TokenServiceInternalUrl), Configuration.GetValue<string>(Config.TokenServicePublicUrl) }
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            UseCors(app);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void UseCors(IApplicationBuilder app)
        {
            app.UseExceptionHandler((builder) => HttpExceptionHandlingExtensions.StartupHandler());
            app.UseCors("default");
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
    }
}
