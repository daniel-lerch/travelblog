using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TravelBlog.Hosting;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseProxy(this IApplicationBuilder app)
    {
        IOptions<HostingOptions> options = app.ApplicationServices.GetRequiredService<IOptions<HostingOptions>>();

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
