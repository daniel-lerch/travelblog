using System;
using System.Collections.Generic;

namespace TravelBlog.Models;

public class MediaViewModel
{
    public MediaViewModel(List<File> files)
    {
        Files = files;
    }

    public List<File> Files { get; }

    public class File
    {
		public File(string name, DateOnly month, string downloadUrl, string thumbnailUrl)
		{
			Name = name;
			Month = month;
			DownloadUrl = downloadUrl;
			ThumbnailUrl = thumbnailUrl;
		}

		public string Name { get; }
        public DateOnly Month { get; }
        public string DownloadUrl { get; }
        public string ThumbnailUrl { get; }
    }
}
