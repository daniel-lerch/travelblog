using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TravelBlog.Database;
using TravelBlog.Extensions;
using TravelBlog.Services;

namespace TravelBlog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureTravelBlog(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new[] { new CultureInfo("de-DE") };

                options.DefaultRequestCulture = new RequestCulture("de-DE");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                options.RequestCultureProviders = new[] { new AcceptLanguageHeaderRequestCultureProvider() };
            });

            services.AddSingleton<ThumbnailService>();
            services.AddDbContext<DatabaseContext>();
            services.AddScoped<AuthenticationService>();
            services.AddScoped<MailingService>();
            services.AddRouting();
            services.AddAuthentication(Constants.AuthCookieScheme)
                .AddCookie(Constants.AuthCookieScheme, options =>
                {
                    options.LoginPath = "/admin/login";
                    options.AccessDeniedPath = "/admin/login";
                    options.ReturnUrlParameter = "redirect";
                });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseMigrations();
            app.UseProxy();

            app.UseRequestLocalization();
            app.UseStatusCodePagesWithReExecute("/status/{0}");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
