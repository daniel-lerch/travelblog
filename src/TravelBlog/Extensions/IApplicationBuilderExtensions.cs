using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TravelBlog.Configuration;

namespace TravelBlog.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseProxy(this IApplicationBuilder app)
        {
            IOptions<ProxyOptions> options = app.ApplicationServices.GetRequiredService<IOptions<ProxyOptions>>();

            if (options.Value.AllowProxies)
            {
                var headersOptions = new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All };
                headersOptions.KnownNetworks.Clear();
                headersOptions.KnownProxies.Clear();
                app.UseForwardedHeaders(headersOptions);
            }

            app.UsePathBase(options.Value.PathBase);

            return app;
        }
    }
}
