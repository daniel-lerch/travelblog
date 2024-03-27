using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Mjml.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TravelBlog.Configuration;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Extensions;

namespace TravelBlog.Services;

public class MimeMessageCreationService
{
    private readonly ILogger<MimeMessageCreationService> logger;
    private readonly IOptions<SiteOptions> siteOptions;
    private readonly IOptions<MailingOptions> mailingOptions;
    private readonly MjmlRenderer renderer = new();

    public MimeMessageCreationService(ILogger<MimeMessageCreationService> logger,
               IOptions<SiteOptions> siteOptions, IOptions<MailingOptions> mailingOptions)
    {
        this.logger = logger;
        this.siteOptions = siteOptions;
        this.mailingOptions = mailingOptions;
    }

    public MimeMessage CreatePostNotification(BlogPost post, Subscriber subscriber, string preview, IUrlHelper urlHelper)
    {
        MimeMessage message = new();
        message.From.Add(new MailboxAddress(mailingOptions.Value.SenderName, mailingOptions.Value.SenderAddress));
        message.To.Add(new MailboxAddress(subscriber.GetName(), subscriber.MailAddress));
        message.ReplyTo.Add(new MailboxAddress(mailingOptions.Value.AuthorName, mailingOptions.Value.AuthorAddress));
        message.Subject = post.Title;
        message.Body = CreateBodyFromTemplate("post", new KeyValuePair<string, string>[]
        {
            new("BLOG_NAME", siteOptions.Value.BlogName),
            new("AUTHOR_NAME", mailingOptions.Value.AuthorName),
            new("POST_TITLE", post.Title),
            new("POST_PREVIEW", preview),
            new("GIVEN_NAME", subscriber.GivenName),
            new("POST_URL", urlHelper.ContentLink("~/post/" + post.Id + "/auth?token=" + subscriber.Token)),
            new("UNSUBSCRIBE_URL", urlHelper.ContentLink("~/unsubscribe?token=" + subscriber.Token)),
        });
        return message;
    }

    private MultipartAlternative CreateBodyFromTemplate(string template, IEnumerable<KeyValuePair<string, string>> variables)
    {
        string textTemplate = LoadResource(template + ".txt");
        string mjmlTemplate = LoadResource(template + ".mjml");

        foreach (KeyValuePair<string, string> variable in variables)
        {
            textTemplate = textTemplate.Replace("${" + variable.Key + "}", variable.Value);
            mjmlTemplate = mjmlTemplate.Replace("${" + variable.Key + "}", variable.Value);
        }

        string html = RenderMjml(mjmlTemplate);

        return
        [
            new TextPart("plain") { Text = textTemplate },
            new TextPart("html") { Text = html }
        ];
    }

    private string RenderMjml(string mjml)
    {
        RenderResult result = renderer.Render(mjml);
        if (result.Errors.Count > 0)
        {
            logger.LogError("MJML rendering failed: {errors}", result.Errors);
        }
        return result.Html;
    }

    private static string LoadResource(string name)
    {
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TravelBlog.Resources." + name)
            ?? throw new ApplicationException($"Resource {name} not found");
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
