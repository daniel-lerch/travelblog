using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBlog.Database.Entities;

namespace TravelBlog.Models
{
    public class PostReadsViewModel
    {
        public PostReadsViewModel(BlogPost post, IReadOnlyList<PostRead> reads)
        {
            Post = post;
            Reads = reads;
        }

        public BlogPost Post { get; }
        public IReadOnlyList<PostRead> Reads { get; }
    }
}
