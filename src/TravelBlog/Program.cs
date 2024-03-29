﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.IO;
using TravelBlog.Database;
using TravelBlog.Extensions;
using TravelBlog.Services;
using TravelBlog.Utilities;

namespace TravelBlog;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureTravelBlog(builder.Configuration);

        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
            options.Secure = CookieSecurePolicy.SameAsRequest;
        });

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var cultures = new[] { new CultureInfo("de-DE") };

            options.DefaultRequestCulture = new RequestCulture("de-DE");
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
            options.RequestCultureProviders = new[] { new AcceptLanguageHeaderRequestCultureProvider() };
        });

        builder.Services.AddSingleton<ThumbnailService>();
        builder.Services.AddSingleton<MarkdownService>();
        builder.Services.AddDbContext<DatabaseContext>();
        builder.Services.AddScoped<AuthenticationService>();

        builder.Services.AddSingleton<JobQueue<EmailDeliveryJobController>>();
        builder.Services.AddHostedService(serviceProvider => serviceProvider.GetRequiredService<JobQueue<EmailDeliveryJobController>>());
        builder.Services.AddScoped<EmailDeliveryService>();

        builder.Services.AddTransient<SubscriberService>();
        builder.Services.AddTransient<MimeMessageCreationService>();

        if (!builder.Environment.IsDevelopment())
        {
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "secrets")));
        }

        builder.Services.AddRouting();
        builder.Services.AddAuthentication(Constants.AuthCookieScheme)
            .AddCookie(Constants.AuthCookieScheme, options =>
            {
                options.LoginPath = "/admin/login";
                options.AccessDeniedPath = "/admin/login";
                options.ReturnUrlParameter = "redirect";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            });

#if DEBUG
        builder.Services.AddMvc().AddRazorRuntimeCompilation();
#else
        builder.Services.AddMvc();
#endif

        var app = builder.Build();

        app.MigrateDatabase();

        if (builder.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseProxy();

        app.UseRequestLocalization();
        app.UseStatusCodePagesWithReExecute("/status/{0}");
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCookiePolicy();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
