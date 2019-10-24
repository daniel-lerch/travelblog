using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Configuration
{
    public class DatabaseOptions
    {
        [Required] public string? ConnectionString { get; set; }
    }
}
