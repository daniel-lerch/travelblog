using System;

namespace TravelBlog.Database.Entities
{
    public class PostRead
    {
        public PostRead(int id, int postId, int subscriberId, DateTime accessTime, string? ipAddress, string? userAgent)
        {
            Id = id;
            PostId = postId;
            SubscriberId = subscriberId;
            AccessTime = accessTime;
            IpAddress = ipAddress;
            UserAgent = userAgent;
        }

        public int Id { get; set; }

        public int PostId { get; set; }
        public BlogPost? Post { get; set; }

        public int SubscriberId { get; set; }
        public Subscriber? Subscriber { get; set; }

        public DateTime AccessTime { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}
