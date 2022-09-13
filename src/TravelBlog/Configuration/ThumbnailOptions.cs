using System;
using System.ComponentModel.DataAnnotations;

namespace TravelBlog.Configuration;

public class ThumbnailOptions
{
    [Range(0, 100)] public int JpegQuality { get; set; }
    [Range(1, 256)] public int Parallelism { get; set; }
}
