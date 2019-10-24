using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Configuration
{
    public class SiteOptions
    {
        [Required] public string? BlogName { get; set; }
        [Required] public string? AdminPassword { get; set; }
    }
}
