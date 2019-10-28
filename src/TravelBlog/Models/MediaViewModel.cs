using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Models
{
    public class MediaViewModel
    {
        public MediaViewModel(List<(string month, string name)> files)
        {
            Files = files;
        }

        public List<(string month, string name)> Files { get; }
    }
}
