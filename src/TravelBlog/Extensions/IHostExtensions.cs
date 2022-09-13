using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TravelBlog.Database;

namespace TravelBlog.Extensions;

public static class IHostExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using (IServiceScope scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            context.Database.Migrate();
        }

        return host;
    }
}
