using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBlog.Database;

namespace TravelBlog.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMigrations(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            using (DatabaseContext context = scope.ServiceProvider.GetService<DatabaseContext>())
            {
                context.Database.EnsureCreated();
            }

            return app;
        }
    }
}
