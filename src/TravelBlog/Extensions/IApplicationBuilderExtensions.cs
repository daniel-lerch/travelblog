﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using TravelBlog.Configuration;
using TravelBlog.Database;

namespace TravelBlog.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMigrations(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            using (DatabaseContext context = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
            {
                context.Database.Migrate();
            }

            return app;
        }

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
