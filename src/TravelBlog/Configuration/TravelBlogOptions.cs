using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Configuration
{
    public class TravelBlogOptions
    {
        public DatabaseOptions DatabaseOptions { get; set; }
        public MailingOptions MailingOptions { get; set; }
        public SiteOptions SiteOptions { get; set; }
    }
}
