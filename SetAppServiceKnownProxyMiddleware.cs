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
    public static class AppServiceKnownProxyMiddleware
    {
        public static void UseAzureAppServiceReverseProxy(this IApplicationBuilder app)
        {
            // WEBSITE_OWNER_NAME is an App Service-supplied Environment Variable
            if(Environment.GetEnvironmentVariable("WEBSITE_OWNER_NAME") != null)
            {
                var options = new ForwardedHeadersOptions {
                    ForwardedHeaders = 
                        ForwardedHeaders.XForwardedFor | 
                        ForwardedHeaders.XForwardedProto   
                };
                options.KnownProxies.Clear();
                options.KnownNetworks.Clear();
                app.UseForwardedHeaders(options);
            }
        }        
    }
}