using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBlog.Configuration;

namespace TravelBlog.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureTravelBlog(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<TravelBlogOptions>()
                .Bind(configuration);
            services.AddOptions<DatabaseOptions>()
                .Bind(configuration.GetSection("Database"))
                .ValidateDataAnnotations();
            services.AddOptions<SiteOptions>()
                .Bind(configuration.GetSection("Site"))
                .ValidateDataAnnotations();

            return services;
        }
    }
}
