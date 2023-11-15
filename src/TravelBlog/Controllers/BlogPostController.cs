using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MimeKit;
using TravelBlog.Configuration;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Extensions;
using TravelBlog.Models;
using TravelBlog.Services;
using UAParser;

namespace TravelBlog.Controllers;

[Route("~/post/{id?}/{action=Index}")]
public class BlogPostController : Controller
{
    private readonly IOptions<SiteOptions> siteOptions;
    private readonly IOptions<MailingOptions> mailingOptions;
    private readonly DatabaseContext database;
    private readonly EmailDeliveryService deliveryService;
    private readonly AuthenticationService authentication;
    private readonly MarkdownService markdown;

    public BlogPostController(IOptions<SiteOptions> siteOptions, IOptions<MailingOptions> mailingOptions, DatabaseContext database,
        EmailDeliveryService deliveryService, AuthenticationService authentication, MarkdownService markdown)
    {
        this.siteOptions = siteOptions;
        this.mailingOptions = mailingOptions;
        this.database = database;
        this.deliveryService = deliveryService;
        this.authentication = authentication;
        this.markdown = markdown;
    }

    [Route("~/posts")]
    [Authorize(Roles = Constants.SubscriberOrAdminRole)]
    public async Task<IActionResult> Index()
    {
        var posts = await database.BlogPosts
            .OrderByDescending(p => p.Id)
            .Select(p => new PostsViewModel.BlogPostPreview(p.Id, p.Title, p.PublishTime, p.Listed, p.Reads!.Count()))
            .ToListAsync();
        return View(new PostsViewModel(posts));
    }

    [Route("~/posts/auth")]
    public async Task<IActionResult> Auth([FromQuery] string token)
    {
        if (await authentication.SignInAsync(HttpContext, token))
            return Redirect("~/posts");
        else
            return StatusCode(403);
    }

    [Authorize(Roles = Constants.SubscriberOrAdminRole)]
    public async Task<IActionResult> Index(int id)
    {
        var post = await database.BlogPosts.Where(p => p.Id == id)
            .Select(p => new { Post = p, Reads = p.Reads!.Count() })
            .SingleOrDefaultAsync();
        if (post is null)
            return StatusCode(404);

        if (HttpContext.User.IsInRole(Constants.SubscriberRole))
        {
            Subscriber? subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Token == HttpContext.User.FindFirstValue("user"));
            if (subscriber is null)
            {
                await authentication.SignOutAsync(HttpContext);
                return StatusCode(403);
            }
            if (!HttpContext.Request.Headers.TryGetValue("User-Agent", out StringValues userAgent))
                return StatusCode(400);
            database.PostReads.Add(new PostRead(id: default, postId: id, subscriberId: subscriber.Id,
                accessTime: DateTime.Now, ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(), userAgent: userAgent.ToString()));
            await database.SaveChangesAsync();
        }

        PostViewModel model = new(post.Post, post.Reads, post.Post.Content.CountWords(), markdown.ToHtml(post.Post.Content));
        return View("Post", model);
    }

    public async Task<IActionResult> Auth(int id, [FromQuery] string token)
    {
        if (await authentication.SignInAsync(HttpContext, token))
            return Redirect("~/post/" + id);
        else
            return StatusCode(403);
    }

    [Authorize(Roles = Constants.AdminRole)]
    public async Task<IActionResult> Reads(int id)
    {
        BlogPost? blog = await database.BlogPosts.SingleOrDefaultAsync(p => p.Id == id);
        if (blog is null)
            return StatusCode(404);

        Parser parser = Parser.GetDefault();

        var raw = await database.PostReads.Where(r => r.PostId == id)
            .Join(database.Subscribers, r => r.SubscriberId, s => s.Id, (r, s) => new { PostRead = r, SubscriberName = s.GetName() })
            .ToListAsync();

        var reads = raw.AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .Select(union =>
            {
                ClientInfo? clientInfo = null;
                if (!string.IsNullOrWhiteSpace(union.PostRead.UserAgent))
                    clientInfo = parser.Parse(union.PostRead.UserAgent);
                return new PostReadsViewModel.Read(union.SubscriberName, union.PostRead.AccessTime, union.PostRead.IpAddress, clientInfo);
            })
            .ToList();

        return View(new PostReadsViewModel(blog, reads));
    }

    [HttpGet("~/post/create")]
    [Authorize(Roles = Constants.AdminRole)]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("~/post/draft")]
    [Authorize(Roles = Constants.AdminRole)]
    public async Task<IActionResult> Draft(string title, string? content, bool listed)
    {
        var post = new BlogPost(id: default, title, content ?? string.Empty, publishTime: default, modifyTime: DateTime.Now, listed);
        database.BlogPosts.Add(post);
        await database.SaveChangesAsync();

        return Redirect("~/post/" + post.Id);
    }

    [HttpPost("~/post/create")]
    [Authorize(Roles = Constants.AdminRole)]
    public async Task<IActionResult> Create(string title, string? content, bool listed)
    {
        var post = new BlogPost(id: default, title, content ?? string.Empty, publishTime: DateTime.Now, modifyTime: default, listed);
        database.BlogPosts.Add(post);
        await database.SaveChangesAsync();

        await NotifySubscribers(post);

        return Redirect("~/post/" + post.Id);
    }

    [HttpGet]
    [Authorize(Roles = Constants.AdminRole)]
    public async Task<IActionResult> Edit(int id)
    {
        BlogPost? post = await database.BlogPosts.SingleOrDefaultAsync(p => p.Id == id);
        if (post is null)
            return StatusCode(404);

        return View(new PostEditViewModel(post));
    }

    [HttpPost]
    [Authorize(Roles = Constants.AdminRole)]
    public async Task<IActionResult> Edit(int id, string title, string? content, bool listed)
    {
        BlogPost? post = await database.BlogPosts.SingleOrDefaultAsync(p => p.Id == id);
        if (post is null)
            return StatusCode(404);

        post.Title = title;
        post.Content = content ?? string.Empty;
        post.ModifyTime = DateTime.Now;
        post.Listed = listed;
        await database.SaveChangesAsync();

        return Redirect("~/post/" + id);
    }

    [HttpPost]
    [Authorize(Roles = Constants.AdminRole)]
    public async Task<IActionResult> Publish(int id, string title, string? content, bool listed)
    {
        BlogPost? post = await database.BlogPosts.SingleOrDefaultAsync(p => p.Id == id);
        if (post is null)
            return StatusCode(404);

        post.Title = title;
        post.Content = content ?? string.Empty;
        post.PublishTime = DateTime.Now;
        post.ModifyTime = default;
        post.Listed = listed;
        await database.SaveChangesAsync();

        await NotifySubscribers(post);

        return Redirect("~/post/" + id);
    }

    private async Task NotifySubscribers(BlogPost post)
    {
        List<Subscriber> subscribers = await database.Subscribers
            .Where(s => s.MailAddress != null && s.ConfirmationTime != default && s.DeletionTime == default).ToListAsync();

        await deliveryService.Enqueue(subscribers.Select(subscriber =>
        {
            string postUrl = Url.ContentLink($"~/post/{post.Id}/auth?token={subscriber.Token}");
            string unsubscribeUrl = Url.ContentLink("~/unsubscribe?token=" + subscriber.Token);
            string message = $"Hey {subscriber.GivenName},\r\n" +
                $"es wurde etwas neues auf {siteOptions.Value.BlogName} gepostet:\r\n" +
                $"{postUrl}\r\n\r\n" +
                $"Du kannst dich von diesem Blog jederzeit hier abmelden: {unsubscribeUrl}";

            MimeMessage mimeMessage = new();
            mimeMessage.From.Add(new MailboxAddress(mailingOptions.Value.SenderName, mailingOptions.Value.SenderAddress));
            mimeMessage.To.Add(new MailboxAddress(subscriber.GetName(), subscriber.MailAddress));
            mimeMessage.ReplyTo.Add(new MailboxAddress(mailingOptions.Value.AuthorName, mailingOptions.Value.AuthorAddress));
            mimeMessage.Subject = "Neuer Post";
            mimeMessage.Body = new TextPart("plain") { Text = message };

            return (subscriber.MailAddress!, mimeMessage);
        }), post.Id);
    }
}
