using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Database.Entities
{
    public class BlogPost
    {
        public BlogPost(int id, string title, string content, DateTime publishTime, DateTime modifyTime)
        {
            Id = id;
            Title = title;
            Content = content;
            PublishTime = publishTime;
            ModifyTime = modifyTime;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishTime { get; set; }
        public DateTime ModifyTime { get; set; }

        public IEnumerable<PostRead>? Reads { get; set; }
    }
}
