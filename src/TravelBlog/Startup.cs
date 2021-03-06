﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
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
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureTravelBlog(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.Secure = CookieSecurePolicy.SameAsRequest;
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

            if (!Environment.IsDevelopment())
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Environment.ContentRootPath, "secrets")));
            }

            services.AddRouting();
            services.AddAuthentication(Constants.AuthCookieScheme)
                .AddCookie(Constants.AuthCookieScheme, options =>
                {
                    options.LoginPath = "/admin/login";
                    options.AccessDeniedPath = "/admin/login";
                    options.ReturnUrlParameter = "redirect";
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                });
#if DEBUG
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddRazorRuntimeCompilation();
#else
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
#endif
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
