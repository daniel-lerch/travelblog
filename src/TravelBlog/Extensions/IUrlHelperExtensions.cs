using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBlog.Extensions
{
    public static class IUrlHelperExtensions
    {
        /// <summary>
        /// Generates an absolute URL for the specified content path.
        /// </summary>
        public static string ContentLink(this IUrlHelper helper, string path)
        {
            HttpRequest request = helper.ActionContext.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{helper.Content(path)}";
        }
    }
}
