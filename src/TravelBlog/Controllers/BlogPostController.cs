using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Models;

namespace TravelBlog.Controllers
{
    [Route("~/post/{action=Index}/{id?}")]
    public class BlogPostController : Controller
    {
        private readonly DatabaseContext database;

        public BlogPostController(DatabaseContext database)
        {
            this.database = database;
        }

        [Route("~/posts")]
        public async Task<IActionResult> Index()
        {
            var posts = await database.BlogPosts
                .Select(p => new PostsViewModel.BlogPostPreview(p.Id, p.Title, p.PublishTime, p.Reads.Count()))
                .ToListAsync();
            return View(new PostsViewModel(posts));
        }

        [Route("~/post/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            PostViewModel model = await database.BlogPosts.Where(p => p.Id == id)
                .Select(p => new PostViewModel(p, p.Reads.Count()))
                .SingleOrDefaultAsync();
            if (model == null)
                return StatusCode(404);

            return View("Post", model);
        }

        [HttpGet]
        [Authorize(Roles = Constants.AdminRole)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.AdminRole)]
        public async Task<IActionResult> Create(string title, string content)
        {
            database.BlogPosts.Add(new BlogPost { Title = title, Content = content, PublishTime = DateTime.Now });
            await database.SaveChangesAsync();
            return Redirect("~/posts");
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
            return Redirect("~/posts");
        }
    }
}