using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Configuration
{
    public class ThumbnailOptions
    {
        [Range(0, 100)] public int JpegQuality { get; set; }
        [Range(1, 256)] public int Parallelism { get; set; }
    }
}
