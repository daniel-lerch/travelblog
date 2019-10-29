using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TravelBlog.Services
{
    public class ThumbnailService
    {
        private readonly SemaphoreSlim semaphore;
        private readonly DirectoryInfo thumbnailDir;
        private readonly DirectoryInfo tempDir;

        public ThumbnailService()
        {
            semaphore = new SemaphoreSlim(Environment.ProcessorCount);
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
                await semaphore.WaitAsync();
                
                try
                {
                    var temp = new FileInfo(Path.Combine(tempDir.FullName, Path.GetRandomFileName()));
                    Image<Rgba32> image = Image.Load<Rgba32>(original.FullName);
                    double factor = (double)size / Math.Max(image.Width, image.Height);

                    // Omit upscaling of images but store the result for caching
                    if (factor < 1.0)
                    {
                        int width = (int)Math.Round(image.Width * factor);
                        int height = (int)Math.Round(image.Height * factor);
                        image.Mutate(x => x.Resize(width, height));
                    }
                    image.Save(temp.FullName, new JpegEncoder { Quality = 85 });
                    temp.MoveTo(thumbnail.FullName, true);
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return thumbnail.FullName;
        }
    }
}
