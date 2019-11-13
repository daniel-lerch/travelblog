using Microsoft.Extensions.Options;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                using (var originalStream = new FileStream(original.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var tempStream = new FileStream(temp.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    await semaphore.WaitAsync();

                    try
                    {
                        CreateThumbnail(originalStream, tempStream, size);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }

                temp.MoveTo(thumbnail.FullName, true);
            }
            return thumbnail.FullName;
        }

        private void CreateThumbnail(FileStream original, FileStream temp, int size)
        {
            SKBitmap? scaled = null;

            try
            {
                using SKBitmap image = SKBitmap.Decode(original);
                int width = image.Width, height = image.Height;
                double factor = (double)size / Math.Max(image.Width, image.Height);

                // Omit upscaling of images but store the result for caching
                if (factor < 1.0)
                {
                    width = (int)Math.Round(image.Width * factor);
                    height = (int)Math.Round(image.Height * factor);
                    scaled = image.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
                }

                using SKImage save = SKImage.FromBitmap(scaled ?? image);
                save.Encode(SKEncodedImageFormat.Jpeg, options.Value.JpegQuality).SaveTo(temp);
            }
            finally
            {
                scaled?.Dispose();
            }
        }
    }
}
