using System;
using TravelBlog.Database.Entities;
using TravelBlog.Extensions;

namespace TravelBlog.Models
{
    public class PostViewModel
    {
        public PostViewModel(BlogPost post, int readCount)
        {
            Post = post;
            ReadCount = readCount;
        }

        public BlogPost Post { get; }
        public int WordCount => Post.Content.CountWords();
        public int ReadCount { get; }
    }
}
