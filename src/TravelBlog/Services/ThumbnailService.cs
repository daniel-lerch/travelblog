using Microsoft.Extensions.Options;
using NetVips;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TravelBlog.Configuration;

namespace TravelBlog.Services
{
    public class ThumbnailService
    {
        private readonly IOptions<ThumbnailOptions> options;
        private readonly SemaphoreSlim semaphore;
        private readonly DirectoryInfo thumbnailDir;
        private readonly DirectoryInfo tempDir;

        public ThumbnailService(IOptions<ThumbnailOptions> options)
        {
            this.options = options;
            semaphore = new SemaphoreSlim(options.Value.Parallelism);
            thumbnailDir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "travelblog", "thumbnails"));
            tempDir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "travelblog", "temp"));
            thumbnailDir.Create();
            tempDir.Create();
        }

        public async Task<string> GetThumbnailAsync(FileInfo original, int size, string month, string file)
        {
            var thumbnail = new FileInfo(Path.Combine(thumbnailDir.FullName, $"{month}_{file}_{size}"));
            if (!thumbnail.Exists)
            {
                var temp = new FileInfo(Path.Combine(tempDir.FullName, Path.GetRandomFileName()));
                await semaphore.WaitAsync();

                try
                {
                    using var thumbnailImage = Image.Thumbnail(original.FullName, size, size, size: Enums.Size.Down);
                    thumbnailImage.Jpegsave(temp.FullName, options.Value.JpegQuality);
                }
                finally
                {
                    semaphore.Release();
                }

                temp.MoveTo(thumbnail.FullName, true);
            }
            return thumbnail.FullName;
        }
    }
}
