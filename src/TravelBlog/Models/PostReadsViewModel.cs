using System;
using System.Collections.Generic;
using TravelBlog.Database.Entities;
using UAParser;

namespace TravelBlog.Models
{
    public class PostReadsViewModel
    {
        public PostReadsViewModel(BlogPost post, IReadOnlyList<Read> reads)
        {
            Post = post;
            Reads = reads;
        }

        public BlogPost Post { get; }
        public IReadOnlyList<Read> Reads { get; }

        public class Read
        {
            public Read(string subscriberName, DateTime accessTime, string? ipAddress, ClientInfo? clientInfo)
            {
                SubscriberName = subscriberName;
                AccessTime = accessTime;
                IpAddress = ipAddress;
                ClientInfo = clientInfo;
            }

            public string SubscriberName { get; }
            public DateTime AccessTime { get; }
            public string? IpAddress { get; }
            public ClientInfo? ClientInfo { get; }
        }
    }
}
