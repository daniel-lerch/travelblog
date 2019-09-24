using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TravelBlog.Configuration;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Extensions;
using TravelBlog.Models;
using TravelBlog.Services;

namespace TravelBlog.Controllers
{
    [Route("~/post/{id?}/{action=Index}")]
    public class BlogPostController : Controller
    {
        private readonly IOptions<SiteOptions> options;
        private readonly ILogger<BlogPostController> logger;
        private readonly DatabaseContext database;
        private readonly MailingService mailer;
        private readonly Services.AuthenticationService authentication;

        public BlogPostController(IOptions<SiteOptions> options, ILogger<BlogPostController> logger,
            DatabaseContext database, MailingService mailer, Services.AuthenticationService authentication)
        {
            this.options = options;
            this.logger = logger;
            this.database = database;
            this.mailer = mailer;
            this.authentication = authentication;
        }

        [Route("~/posts")]
        [Authorize(Roles = Constants.SubscriberOrAdminRole)]
        public async Task<IActionResult> Index()
        {
            var posts = await database.BlogPosts
                .Select(p => new PostsViewModel.BlogPostPreview(p.Id, p.Title, p.PublishTime, p.Reads.Count()))
                .ToListAsync();
            return View(new PostsViewModel(posts));
        }

        [Route("~/posts/auth")]
        public async Task<IActionResult> Auth([FromQuery] string token)
        {
            if (await authentication.SignInAsnyc(HttpContext, token))
                return Redirect("~/posts");
            else
                return StatusCode(403);
        }

        [Authorize(Roles = Constants.SubscriberOrAdminRole)]
        public async Task<IActionResult> Index(int id)
        {
            PostViewModel model = await database.BlogPosts.Where(p => p.Id == id)
                .Select(p => new PostViewModel(p, p.Reads.Count()))
                .SingleOrDefaultAsync();
            if (model == null)
                return StatusCode(404);
            if (HttpContext.User.IsInRole(Constants.SubscriberRole))
            {
                Subscriber subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Token == HttpContext.User.FindFirstValue("user"));
                if (subscriber == null)
                {
                    await HttpContext.SignOutAsync(Constants.AuthCookieScheme);
                    return StatusCode(403);
                }
                database.PostReads.Add(new PostRead { PostId = id, SubscriberId = subscriber.Id, AccessTime = DateTime.Now });
                await database.SaveChangesAsync();
            }

            return View("Post", model);
        }

        public async Task<IActionResult> Auth(int id, [FromQuery] string token)
        {
            if (await authentication.SignInAsnyc(HttpContext, token))
                return Redirect("~/post/" + id);
            else
                return StatusCode(403);
        }

        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> Reads(int id)
        {
            BlogPost blog = await database.BlogPosts.SingleOrDefaultAsync(p => p.Id == id);
            if (blog == null)
                return StatusCode(404);

            List<PostRead> reads = await database.PostReads.Where(r => r.PostId == id)
                .Include(r => r.Subscriber).ToListAsync();

            return View(new PostReadsViewModel(blog, reads));
        }

        [HttpGet("~/post/create")]
        [Authorize(Roles = Constants.AdminRole)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("~/post/create")]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> Create(string title, string content)
        {
            var post = new BlogPost { Title = title, Content = content, PublishTime = DateTime.Now };
            database.BlogPosts.Add(post);
            await database.SaveChangesAsync();

            List<Subscriber> subscribers = await database.Subscribers.Where(s => s.ConfirmationTime != default).ToListAsync();
            foreach (Subscriber subscriber in subscribers)
            {
                string postUrl = Url.ContentLink($"~/post/{post.Id}/auth?token={subscriber.Token}");
                string message = $"Hey {subscriber.GivenName},\r\n" +
                    $"es wurde etwas neues auf {options.Value.BlogName} gepostet:\r\n" +
                    $"{postUrl}";
                string unsubscribe = Url.ContentLink("~/unsubscribe?token=" + subscriber.Token);
                try
                {
                    await mailer.SendMailAsync(subscriber.GetName(), subscriber.MailAddress, "Neuer Post", message, unsubscribe);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to send mail to {subscriber.MailAddress}");
                }
            }

            return Redirect("~/post/" + post.Id);
        }

        [HttpGet]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> Edit(int id)
        {
            BlogPost blog = await database.BlogPosts.SingleOrDefaultAsync(p => p.Id == id);
            if (blog == null)
                return StatusCode(404);

            return View(new EditPostViewModel(blog.Title, blog.Content));
        }
        
        [HttpPost]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> Edit(int id, string title, string content)
        {
            BlogPost blog = await database.BlogPosts.SingleOrDefaultAsync(p => p.Id == id);
            if (blog == null)
                return StatusCode(404);

            blog.Title = title;
            blog.Content = content;
            blog.ModifyTime = DateTime.Now;
            await database.SaveChangesAsync();
            return Redirect("~/post/" + id);
        }
    }
}