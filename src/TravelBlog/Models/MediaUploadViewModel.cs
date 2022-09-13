using System.Collections.Generic;

namespace TravelBlog.Models;

public class MediaUploadViewModel
{
    public MediaUploadViewModel(List<(string name, bool success)> files)
    {
        Files = files;
    }

    public List<(string name, bool success)> Files { get; }
}
