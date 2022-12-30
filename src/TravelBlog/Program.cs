using Microsoft.AspNetCore.Builder;
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
using TravelBlog.Database.Entities;
using TravelBlog.Extensions;
using TravelBlog.Hosting;
using TravelBlog.Services;
using TravelBlog.Services.LightJobManager;

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
		builder.Services.AddScoped<MailingService>();

		builder.Services.AddSingleton<JobSchedulerService<MailJob, MailJobContext>>();
		builder.Services.AddHostedService(provider => provider.GetRequiredService<JobSchedulerService<MailJob, MailJobContext>>());

		if (builder.Environment.IsDevelopment())
		{
			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
				{
					builder.AllowAnyOrigin();
					builder.AllowAnyMethod();
					builder.AllowAnyHeader();
				});
			});
		}
		else
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

		if (builder.Environment.IsDevelopment())
		{
			app.UseCors();
		}

		app.UseCookiePolicy();
		app.UseAuthentication();
		app.UseAuthorization();
		app.UseEndpoints(endpoints => endpoints.MapControllers());
		app.UseSpa(spa => spa.UseVueSpaFileProvider());
		app.Run();
	}
}
