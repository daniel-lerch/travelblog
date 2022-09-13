using System;
using System.Collections.Generic;

namespace TravelBlog.Models;

public class PostsViewModel
{
    public PostsViewModel(IReadOnlyList<BlogPostPreview> posts)
    {
        Posts = posts;
    }

    public IReadOnlyList<BlogPostPreview> Posts { get; }

    public class BlogPostPreview
    {
        public BlogPostPreview(int id, string title, DateTime publishTime, bool listed, int readCount)
        {
            Id = id;
            Title = title;
            PublishTime = publishTime;
            Listed = listed;
            ReadCount = readCount;
        }

        public int Id { get; }
        public string Title { get; }
        public DateTime PublishTime { get; }
        public bool Listed { get; }
        public int ReadCount { get; }
    }
}
