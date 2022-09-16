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
using TravelBlog.Models;
using TravelBlog.Services;

namespace TravelBlog.Controllers;

[Route("~/admin/{action=Index}")]
public class AdminController : Controller
{
    private readonly IOptions<SiteOptions> options;
    private readonly DatabaseContext database;
    private readonly AuthenticationService authentication;
    private readonly SubscriberService subscriberService;

    public AdminController(IOptions<SiteOptions> options, DatabaseContext database,
        AuthenticationService authentication, SubscriberService subscriberService)
    {
        this.options = options;
        this.database = database;
        this.authentication = authentication;
        this.subscriberService = subscriberService;
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
        if (await subscriberService.Confirm(id, Url))
            return Redirect("~/admin?status=success");
        else
            return Redirect("~/admin?status=error");
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
