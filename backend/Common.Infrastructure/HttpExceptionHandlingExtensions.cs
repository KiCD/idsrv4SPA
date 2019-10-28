using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace TB.TokenService.Infrastructure
{
    public static class HttpExceptionHandlingExtensions
    {
        public static Action<IApplicationBuilder> StartupHandler(ILogger logger = null)
        {
            return (appError =>
            {
                appError.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*"); // required for CORS response type detection in frontend
                    if (contextFeature != null)
                    {
                        if (contextFeature.Error is InvalidOperationException)
                        {
                            logger?.LogError("Raised invalid operation exception", contextFeature.Error);
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        await context.Response.WriteAsync(new
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Invalid authorization exception"
                        }.ToString());
                    }
                });
            });
        }
    }
}
