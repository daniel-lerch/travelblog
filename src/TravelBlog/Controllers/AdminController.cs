﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
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

namespace TravelBlog.Controllers
{
    [Route("~/admin/{action=Index}")]
    public class AdminController : Controller
    {
        private readonly IOptions<SiteOptions> options;
        private readonly DatabaseContext database;
        private readonly MailingService mailer;

        public AdminController(IOptions<SiteOptions> options, DatabaseContext database, MailingService mailer)
        {
            this.options = options;
            this.database = database;
            this.mailer = mailer;
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
                var claims = new[] { new Claim("user", username), new Claim("role", Constants.AdminRole) };
                await HttpContext.SignInAsync(Constants.AuthCookieScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

                if (Url.IsLocalUrl(redirect))
                    return Redirect(redirect);
                else
                    return Redirect("~/");
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Constants.AuthCookieScheme);

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
            Subscriber subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Id == id);
            if (subscriber == null)
                return Redirect("~/admin?status=error");
            subscriber.ConfirmationTime = DateTime.Now;
            await database.SaveChangesAsync();

            string mail;
            if (await database.BlogPosts.AnyAsync()) 
                // Give late subscribers the chance to view posts before they get the next notification mail.
                mail = $"Hey {subscriber.GivenName},\r\n" +
                $"du hast dich erfolgreich bei {options.Value.BlogName} registriert.\r\n" +
                $"Es wurden bereits Posts veröffentlicht: {Url.ContentLink($"~/posts/auth?token={subscriber.Token}")}";
            else
                mail = $"Hey {subscriber.GivenName},\r\n" +
                $"du hast dich erfolgreich bei {options.Value.BlogName} registriert.\r\n" +
                $"Ab sofort wirst du per E-Mail über neue Einträge informiert.";

            string url = Url.ContentLink("~/unsubscribe?token=" + subscriber.Token);
            await mailer.SendMailAsync(subscriber.GetName(), subscriber.MailAddress, "Erfolgreich registriert", mail, url);

            return Redirect("~/admin?status=success");
        }

        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            Subscriber subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Id == id);
            if (subscriber == null)
                return Redirect("~/admin?status=error");
            database.Subscribers.Remove(subscriber);
            await database.SaveChangesAsync();
            return Redirect("~/admin?status=success");
        }
    }
}
