using Microsoft.Extensions.Options;
using SkiaSharp;
using System;
using System.Collections.Generic;
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
            using SKCodec codec = SKCodec.Create(original);
            using SKBitmap image = SKBitmap.Decode(codec);

            int width = image.Width, height = image.Height;
            double factor = (double)size / Math.Max(image.Width, image.Height);

            // Omit upscaling of images but store the result for caching
            if (factor < 1.0)
            {
                width = (int)Math.Round(image.Width * factor);
                height = (int)Math.Round(image.Height * factor);
            }

            Action<SKCanvas> transform = Rotate(codec.EncodedOrigin, ref width, ref height);

            var info = new SKImageInfo(width, height);
            using SKSurface surface = SKSurface.Create(info);

            transform(surface.Canvas);

            using (var paint = new SKPaint { IsAntialias = true, FilterQuality = SKFilterQuality.High })
            {
                surface.Canvas.DrawBitmap(image, info.Rect, paint);
            }

            surface.Canvas.Flush();

            using SKImage save = surface.Snapshot();
            save.Encode(SKEncodedImageFormat.Jpeg, options.Value.JpegQuality).SaveTo(temp);
        }

        // Unfortunately we cannot copy all exif meta data: https://github.com/mono/SkiaSharp/issues/836#issuecomment-584895517
        private Action<SKCanvas> Rotate(SKEncodedOrigin origin, ref int width, ref int height)
        {
            Action<SKCanvas> transform;
            int newWidth = width;
            int newHeight = height;

            switch (origin)
            {
                case SKEncodedOrigin.TopLeft:
                    transform = canvas => { };
                    break;
                case SKEncodedOrigin.TopRight:
                    // flip along the x-axis
                    transform = canvas => canvas.Scale(-1, 1, newWidth / 2, newHeight / 2);
                    break;
                case SKEncodedOrigin.BottomRight:
                    transform = canvas => canvas.RotateDegrees(180, newWidth / 2, newHeight / 2);
                    break;
                case SKEncodedOrigin.BottomLeft:
                    // flip along the y-axis
                    transform = canvas => canvas.Scale(1, -1, newWidth / 2, newHeight / 2);
                    break;
                case SKEncodedOrigin.LeftTop:
                    newWidth = height;
                    newHeight = width;
                    transform = canvas =>
                    {
                        // Rotate 90
                        canvas.RotateDegrees(90, newWidth / 2, newHeight / 2);
                        canvas.Scale((float)newHeight / newWidth, (float)-newWidth / newHeight, newWidth / 2, newHeight / 2);
                    };
                    break;
                case SKEncodedOrigin.RightTop:
                    newWidth = height;
                    newHeight = width;
                    transform = canvas =>
                    {
                        // Rotate 90
                        canvas.RotateDegrees(90, newWidth / 2, newHeight / 2);
                        canvas.Scale((float)newHeight / newWidth, (float)newWidth / newHeight, newWidth / 2, newHeight / 2);
                    };
                    break;
                case SKEncodedOrigin.RightBottom:
                    newWidth = height;
                    newHeight = width;
                    transform = canvas =>
                    {
                        // Rotate 90
                        canvas.RotateDegrees(90, newWidth / 2, newHeight / 2);
                        canvas.Scale((float)-newHeight / newWidth, (float)newWidth / newHeight, newWidth / 2, newHeight / 2);
                    };
                    break;
                case SKEncodedOrigin.LeftBottom:
                    newWidth = height;
                    newHeight = width;
                    transform = canvas =>
                    {
                        // Rotate 90
                        canvas.RotateDegrees(90, newWidth / 2, newHeight / 2);
                        canvas.Scale((float)-newHeight / newWidth, (float)-newWidth / newHeight, newWidth / 2, newHeight / 2);
                    };
                    break;
                default:
                    throw new NotSupportedException();
            }

            width = newWidth;
            height = newHeight;
            return transform;
        }
    }
}
