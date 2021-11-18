using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using TravelBlog.Configuration;

namespace TravelBlog.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureTravelBlog(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<DatabaseOptions>()
                .Bind(configuration.GetSection("Database"))
                .ValidateDataAnnotations();
            services.AddOptions<MailingOptions>()
                .Bind(configuration.GetSection("Mailing"))
                .Validate(options =>
                {
                    if (!options.EnableMailing) return true;
                    ValidationContext context = new ValidationContext(options);
                    return Validator.TryValidateObject(options, context, null);
                });
            services.AddOptions<ProxyOptions>()
                .Bind(configuration.GetSection("Proxy"))
                .ValidateDataAnnotations();
            services.AddOptions<SiteOptions>()
                .Bind(configuration.GetSection("Site"))
                .ValidateDataAnnotations();
            services.AddOptions<ThumbnailOptions>()
                .Bind(configuration.GetSection("Thumbnail"))
                .ValidateDataAnnotations();

            return services;
        }
    }
}
