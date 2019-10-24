using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Database.Entities
{
    public class PostRead
    {
        public PostRead(int id, int postId, int subscriberId, DateTime accessTime)
        {
            Id = id;
            PostId = postId;
            SubscriberId = subscriberId;
            AccessTime = accessTime;
        }

        public int Id { get; set; }

        public int PostId { get; set; }
        public BlogPost? Post { get; set; }

        public int SubscriberId { get; set; }
        public Subscriber? Subscriber { get; set; }

        public DateTime AccessTime { get; set; }
    }
}
