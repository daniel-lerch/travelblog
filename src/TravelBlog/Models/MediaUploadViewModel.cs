using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Models
{
    public class MediaUploadViewModel
    {
        public MediaUploadViewModel(List<(string name, bool success)> files)
        {
            Files = files;
        }

        public List<(string name, bool success)> Files { get; }
    }
}
