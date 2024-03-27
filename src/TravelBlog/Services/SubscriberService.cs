using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using TravelBlog.Configuration;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Extensions;
using Wiry.Base32;

namespace TravelBlog.Services;

public class SubscriberService
{
    private readonly IOptions<SiteOptions> siteOptions;
    private readonly IOptions<MailingOptions> mailingOptions;
    private readonly DatabaseContext database;
    private readonly EmailDeliveryService deliveryService;

    public SubscriberService(IOptions<SiteOptions> siteOptions, IOptions<MailingOptions> mailingOptions, DatabaseContext database, EmailDeliveryService deliveryService)
    {
        this.siteOptions = siteOptions;
        this.mailingOptions = mailingOptions;
        this.database = database;
        this.deliveryService = deliveryService;
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
        if (subscriber == null || subscriber.MailAddress == null || subscriber.ConfirmationTime != default || subscriber.DeletionTime != default)
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

        MimeMessage message = new();
        message.From.Add(new MailboxAddress(mailingOptions.Value.SenderName, mailingOptions.Value.SenderAddress));
        message.To.Add(new MailboxAddress(subscriber.GetName(), subscriber.MailAddress));
        message.ReplyTo.Add(new MailboxAddress(mailingOptions.Value.AuthorName, mailingOptions.Value.AuthorAddress));
        message.Subject = "Erfolgreich registriert";
        message.Body = new TextPart("plain") { Text = mail };
        
        await deliveryService.Enqueue(subscriber.MailAddress, message, null);

        return true;
    }

    private static string RandomToken()
    {
        byte[] buffer = new byte[20];
        RandomNumberGenerator.Fill(buffer);
        return Base32Encoding.Standard.GetString(buffer);
    }
}
