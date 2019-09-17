using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBlog.Database.Entities;

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
        public int ReadCount { get; }
    }
}
