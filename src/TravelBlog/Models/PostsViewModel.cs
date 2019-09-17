using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBlog.Database.Entities;

namespace TravelBlog.Models
{
    public class PostsViewModel
    {
        public PostsViewModel(IReadOnlyList<BlogPostPreview> posts)
        {
            Posts = posts;
        }

        public IReadOnlyList<BlogPostPreview> Posts { get; }

        public class BlogPostPreview
        {
            public BlogPostPreview(int id, string title, DateTime publishTime, int readCount)
            {
                Id = id;
                Title = title;
                PublishTime = publishTime;
                ReadCount = readCount;
            }

            public int Id { get; }
            public string Title { get; }
            public DateTime PublishTime { get; set; }
            public int ReadCount { get; set; }
        }
    }
}
