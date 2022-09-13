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
using TravelBlog.Services;
using static TravelBlog.Constants;

namespace TravelBlog.Controllers;

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
        var extensions = new[] { ".jpg", ".jpeg", ".png", ".mp4" };
        var folder = new DirectoryInfo(Path.Combine(environment.ContentRootPath, "media"));

        foreach (DirectoryInfo month in folder.EnumerateDirectories().OrderByDescending(x => x.Name))
        {
            foreach (FileInfo file in month.EnumerateFiles().OrderByDescending(x => x.Name))
            {
                if (extensions.Contains(file.Extension.ToLowerInvariant()))
                {
                    result.Add((month.Name, file.Name));
                }
            }
        }

        return View("Index", new MediaViewModel(result));
    }

    [HttpPost]
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

    [HttpGet]
    [HttpHead]
    [HttpOptions]
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
        if (!mimeProvider.TryGetContentType(fileInfo.Name, out string? contentType))
        {
            contentType = "application/octet-stream";
        }

        if (size == 0 || !extensions.Contains(fileInfo.Extension.ToLowerInvariant()))
        {
            return PhysicalFile(fileInfo.FullName, contentType, enableRangeProcessing: true);
        }
        else
        {
            return PhysicalFile(await thumbnail.GetThumbnailAsync(fileInfo, size, month, file), contentType, enableRangeProcessing: true);
        }
    }

    [HttpDelete]
    [Route("~/media/{month}/{file}")]
    [Authorize(Roles = AdminRole)]
    public IActionResult Media(string month, string file)
    {
        var fileInfo = new FileInfo(Path.Combine(environment.ContentRootPath, "media", month, file));
        try
        {
            fileInfo.Delete();
            return StatusCode(204);
        }
        catch (FileNotFoundException)
        {
            return StatusCode(404);
        }
        catch (IOException)
        {
            return StatusCode(500);
        }
        catch (System.Security.SecurityException) // no permission to delete the file
        {
            return StatusCode(403);
        }
        catch (UnauthorizedAccessException) // attempting to delete a directory
        {
            return StatusCode(400);
        }
    }
}
