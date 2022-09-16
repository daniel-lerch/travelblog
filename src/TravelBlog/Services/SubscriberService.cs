using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelBlog.Configuration;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Extensions;
using TravelBlog.Services.LightJobManager;
using Wiry.Base32;

namespace TravelBlog.Services;

public class SubscriberService
{
    private readonly IOptions<SiteOptions> siteOptions;
    private readonly DatabaseContext database;
    private readonly JobSchedulerService<MailJob, MailJobContext> mailScheduler;

    public SubscriberService(IOptions<SiteOptions> siteOptions, DatabaseContext database, JobSchedulerService<MailJob, MailJobContext> mailScheduler)
    {
        this.siteOptions = siteOptions;
        this.database = database;
        this.mailScheduler = mailScheduler;
    }

    public async ValueTask<bool> Register(string mailAddress, string givenName, string familyName)
    {
        database.Subscribers.Add(new(mailAddress, givenName, familyName, RandomToken()));
        try
        {
            await database.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            return false;
        }
    }

    public async ValueTask<bool> Confirm(int id, IUrlHelper urlHelper)
    {
        Subscriber? subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Id == id);
        if (subscriber == null || subscriber.ConfirmationTime != default || subscriber.DeletionTime != default)
            return false;
        subscriber.ConfirmationTime = DateTime.Now;
        await database.SaveChangesAsync();

        string mail = $"Hey {subscriber.GivenName},\r\n" +
            $"du hast dich erfolgreich bei {siteOptions.Value.BlogName} registriert.\r\n";

        int publishedPosts = await database.BlogPosts.CountAsync(p => p.PublishTime != default && p.Listed);
        if (publishedPosts > 0)
            // Give late subscribers the chance to view posts before they get the next notification mail.
            mail += $"Es wurden bereits {publishedPosts} Posts veröffentlicht: {urlHelper.ContentLink($"~/posts/auth?token={subscriber.Token}")}\r\n";
        else
            mail += $"Ab sofort wirst du per E-Mail über neue Einträge informiert.\r\n";

        mail += $"\r\nDu kannst dich von diesem Blog jederzeit hier abmelden: {urlHelper.ContentLink("~/unsubscribe?token=" + subscriber.Token)}";

        await mailScheduler.Enqueue(new MailJob(default, subscriber.Id, subject: "Erfolgreich registriert", mail));

        return true;
    }

    private static string RandomToken()
    {
        byte[] buffer = new byte[20];
        RandomNumberGenerator.Fill(buffer);
        return Base32Encoding.Standard.GetString(buffer);
    }
}
