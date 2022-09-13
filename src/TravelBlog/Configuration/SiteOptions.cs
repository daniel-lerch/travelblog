﻿using System.ComponentModel.DataAnnotations;

namespace TravelBlog.Configuration;

public class SiteOptions
{
    [Required] public string? BlogName { get; set; }
    [Required] public string? AdminPassword { get; set; }
    public bool EnableDebugFeatures { get; set; }
}
