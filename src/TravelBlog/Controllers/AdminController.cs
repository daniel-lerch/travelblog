using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TravelBlog.Configuration;

namespace TravelBlog.Controllers
{
    [Route("~/admin/{action=Index}")]
    public class AdminController : Controller
    {
        private readonly IOptions<SiteOptions> options;

        public AdminController(IOptions<SiteOptions> options)
        {
            this.options = options;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, [FromQuery] string redirect)
        {
            if (password == options.Value.AdminPassword)
            {
                var claims = new[] { new Claim("user", username), new Claim("role", "Admin") };
                await HttpContext.SignInAsync(Constants.AdminCookieScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));

                if (Url.IsLocalUrl(redirect))
                    return Redirect(redirect);
                else
                    return Redirect("~/");
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(Constants.AdminCookieScheme);

            return Redirect("~/");
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
