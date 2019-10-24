using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TravelBlog.Configuration
{
    public class ProxyOptions
    {
        public string? PathBase { get; set; }
        public bool AllowProxies { get; set; }
    }
}
