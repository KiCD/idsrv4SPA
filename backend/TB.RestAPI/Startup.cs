using System;
using Common.Infrastructure.Configuration;
using Common.Infrastructure.Logging;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TB.RestAPI.Configuration;

namespace TB.RestAPI
{
    public class Startup
    {
        protected LoggingLevels LoggingLevels { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LoggingLevels = configuration.GetLoggingLevels();
            Log.Logger = SerilogConfiguration.CreateSerilogLogger("RestApi",LoggingLevels);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAuthorization(services);
            AddCors(services);
            services.AddControllers();
        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(options =>
            {
                // base-address of your identityserver
                options.Authority = Configuration.GetValue<string>(Config.TokenServiceUrl);

                // name of the API resource
                options.ApiName = "documentapi";
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
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
