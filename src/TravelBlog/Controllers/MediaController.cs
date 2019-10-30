﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using TravelBlog.Models;
using TravelBlog.Services;
using static TravelBlog.Constants;

namespace TravelBlog.Controllers
{
    [Route("~/media/{action=Index}")]
    public class MediaController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private readonly ThumbnailService thumbnail;

        public MediaController(IWebHostEnvironment environment, ThumbnailService thumbnail)
        {
            this.environment = environment;
            this.thumbnail = thumbnail;
        }

        [Authorize(Roles = AdminRole)]
        public IActionResult Index()
        {
            var result = new List<(string month, string name)>();
            var extensions = new[] { ".jpg", ".jpeg", ".png" };
            var folder = new DirectoryInfo(Path.Combine(environment.ContentRootPath, "media"));
            DirectoryInfo[] months = folder.GetDirectories();
            for (int i = months.Length - 1; i >= 0; i--)
            {
                FileInfo[] files = months[i].GetFiles();
                for (int j = files.Length - 1; j >= 0; j--)
                {
                    if (extensions.Contains(files[j].Extension))
                    {
                        result.Add((months[i].Name, files[j].Name));
                    }
                }
            }

            return View("Index", new MediaViewModel(result));
        }

        [HttpGet()]
        [Authorize(Roles = AdminRole)]
        public IActionResult Upload()
        {
            return View("Upload");
        }

        [HttpPost()]
        [DisableRequestSizeLimit]
        [Authorize(Roles = AdminRole)]
        public async Task<IActionResult> Upload(IFormFileCollection files)
        {
            var folder = new DirectoryInfo(Path.Combine(environment.ContentRootPath, "media", DateTime.Now.ToString("yyMM")));
            folder.Create();
            var status = new List<(string name, bool success)>(capacity: files.Count);

            foreach (IFormFile file in files)
            {
                FileInfo physical = new FileInfo(Path.Combine(folder.FullName, file.FileName));
                FileStream? stream = null;
                try
                {
                    stream = new FileStream(physical.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                    await file.CopyToAsync(stream);
                    status.Add((file.FileName, true));
                }
                catch (IOException)
                {
                    status.Add((file.FileName, false));
                }
                finally
                {
                    if (stream != null) await stream.DisposeAsync();
                }
            }

            return View("Uploaded", new MediaUploadViewModel(status));
        }

        [Route("~/media/{month}/{file}")]
        [Authorize(Roles = SubscriberOrAdminRole)]
        public async Task<IActionResult> Media(string month, string file, [FromQuery] int size)
        {
            var extensions = new[] { ".jpg", ".jpeg" };
            var fileInfo = new FileInfo(Path.Combine(environment.ContentRootPath, "media", month, file));
            if (!fileInfo.Exists)
                return StatusCode(404);

            size = size switch
            {
                400 => 400,
                1600 => 1600,
                _ => 0
            };

            var mimeProvider = new FileExtensionContentTypeProvider();
            if (!mimeProvider.TryGetContentType(fileInfo.Name, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            if (size == 0 || !extensions.Contains(fileInfo.Extension))
            {
                return PhysicalFile(fileInfo.FullName, contentType, enableRangeProcessing: true);
            }
            else
            {
                return PhysicalFile(await thumbnail.GetThumbnailAsync(fileInfo, size, month, file), contentType, enableRangeProcessing: true);
            }
        }
    }
}
