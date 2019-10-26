using System;
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
using static TravelBlog.Constants;

namespace TravelBlog.Controllers
{
    [Route("~/media/{action=Index}")]
    public class MediaController : Controller
    {
        private readonly IWebHostEnvironment environment;

        public MediaController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        [Authorize(Roles = AdminRole)]
        public IActionResult Index()
        {
            return View("Index");
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
        public IActionResult Media(string month, string file, [FromQuery] string? width)
        {
            var fileInfo = new FileInfo(Path.Combine(environment.ContentRootPath, "media", month, file));
            if (!fileInfo.Exists)
                return StatusCode(404);

            var mimeProvider = new FileExtensionContentTypeProvider();
            if (!mimeProvider.TryGetContentType(fileInfo.Name, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            return PhysicalFile(fileInfo.FullName, contentType, enableRangeProcessing: true);
        }
    }
}