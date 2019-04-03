using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IsHttpBug
{
    public class SetAppServiceKnownProxyMiddlewareFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<SetAppServiceKnownProxyMiddleware>(builder);
                next(builder);
            };
        }
    }

    public class SetAppServiceKnownProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _app;
        private readonly ILogger<SetAppServiceKnownProxyMiddleware> _logger;

        public SetAppServiceKnownProxyMiddleware(RequestDelegate next, 
            IApplicationBuilder app,
            ILogger<SetAppServiceKnownProxyMiddleware> logger)
        {
            _next = next;
            _app = app;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // WEBSITE_OWNER_NAME is an App Service-supplied Environment Variable
            if(Environment.GetEnvironmentVariable("WEBSITE_OWNER_NAME") != null)
            {
                _logger.LogDebug($"[AppSvc] Remote IP: {httpContext.Connection.RemoteIpAddress.ToString()}");

                var proxy = httpContext.Connection.RemoteIpAddress.ToString();
                var ip = IPAddress.Parse(proxy);

                _logger.LogDebug($"[AppSvc] Proxy to Add: {ip.ToString()}");

                var options = new ForwardedHeadersOptions {
                    ForwardedHeaders = 
                        ForwardedHeaders.XForwardedFor | 
                        ForwardedHeaders.XForwardedProto    
                };
                
                try
                {
                    options.KnownProxies.Add(ip);
                    _app.UseForwardedHeaders(options);
                    _logger.LogDebug($"[AppSvc] Added proxy: {ip.ToString()}");
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "[AppSvc] Error adding proxy");
                }
            }

            await _next(httpContext);
        }
    }
}