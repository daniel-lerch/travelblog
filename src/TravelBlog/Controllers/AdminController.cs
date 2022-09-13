using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelBlog.Configuration;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Extensions;
using TravelBlog.Models;
using TravelBlog.Services;
using TravelBlog.Services.LightJobManager;

namespace TravelBlog.Controllers;

[Route("~/admin/{action=Index}")]
public class AdminController : Controller
{
    private readonly IOptions<SiteOptions> options;
    private readonly DatabaseContext database;
    private readonly JobSchedulerService<MailJob, MailJobContext> scheduler;
    private readonly AuthenticationService authentication;

    public AdminController(IOptions<SiteOptions> options, DatabaseContext database,
        JobSchedulerService<MailJob, MailJobContext> scheduler, AuthenticationService authentication)
    {
        this.options = options;
        this.database = database;
        this.scheduler = scheduler;
        this.authentication = authentication;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password, [FromQuery] string redirect)
    {
        if (password == options.Value.AdminPassword)
        {
            await authentication.SignInAsync(HttpContext, username, Constants.AdminRole);

            if (Url.IsLocalUrl(redirect))
                return Redirect(redirect);
            else
                return Redirect("~/");
        }

        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await authentication.SignOutAsync(HttpContext);

        return Redirect("~/");
    }

    [Authorize(Roles = Constants.AdminRole)]
    public async Task<IActionResult> Index([FromQuery] string status)
    {
        var pending = await database.Subscribers.Where(s => s.ConfirmationTime == default).OrderBy(s => s.FamilyName).ToListAsync();
        var confirmed = await database.Subscribers.Where(s => s.ConfirmationTime != default).OrderBy(s => s.FamilyName).ToListAsync();
        return View(new AdminViewModel(pending, confirmed, status));
    }

    [Authorize(Roles = Constants.AdminRole)]
    public async Task<IActionResult> Confirm([FromQuery] int id)
    {
        Subscriber? subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Id == id);
        if (subscriber is null || subscriber.ConfirmationTime != default || subscriber.DeletionTime != default)
            return Redirect("~/admin?status=error");
        subscriber.ConfirmationTime = DateTime.Now;
        await database.SaveChangesAsync();

        string mail = $"Hey {subscriber.GivenName},\r\n" +
            $"du hast dich erfolgreich bei {options.Value.BlogName} registriert.\r\n";

        if (await database.BlogPosts.AnyAsync(p => p.PublishTime != default))
            // Give late subscribers the chance to view posts before they get the next notification mail.
            mail += $"Es wurden bereits Posts veröffentlicht: {Url.ContentLink($"~/posts/auth?token={subscriber.Token}")}\r\n";
        else
            mail += $"Ab sofort wirst du per E-Mail über neue Einträge informiert.\r\n";

        mail += $"\r\nDu kannst dich von diesem Blog jederzeit hier abmelden: {Url.ContentLink("~/unsubscribe?token=" + subscriber.Token)}";

        await scheduler.Enqueue(new MailJob(default, subscriber.Id, subject: "Erfolgreich registriert", mail));

        return Redirect("~/admin?status=success");
    }

    [Authorize(Roles = Constants.AdminRole)]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        Subscriber? subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Id == id);
        if (subscriber is null || subscriber.DeletionTime != default)
            return Redirect("~/admin?status=error");
        if (subscriber.ConfirmationTime == default)
        {
            database.Subscribers.Remove(subscriber);
        }
        else
        {
            subscriber.MailAddress = null;
            subscriber.DeletionTime = DateTime.Now;
            subscriber.Token = null;
        }
        await database.SaveChangesAsync();
        return Redirect("~/admin?status=success");
    }
}
