using System;
using System.Collections.Generic;

namespace TravelBlog.Database.Entities
{
    public class BlogPost
    {
        public BlogPost(int id, string title, string content, DateTime publishTime, DateTime modifyTime, bool listed)
        {
            Id = id;
            Title = title;
            Content = content;
            PublishTime = publishTime;
            ModifyTime = modifyTime;
            Listed = listed;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishTime { get; set; }
        public DateTime ModifyTime { get; set; }
        public bool Listed { get; set; }

        public IEnumerable<PostRead>? Reads { get; set; }
    }
}
