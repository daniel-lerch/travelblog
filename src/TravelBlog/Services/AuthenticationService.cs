using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TravelBlog.Database;
using TravelBlog.Database.Entities;

namespace TravelBlog.Services
{
    public class AuthenticationService
    {
        private readonly DatabaseContext database;

        public AuthenticationService(DatabaseContext database)
        {
            this.database = database;
        }

        public async Task<bool> SignInAsnyc(HttpContext context, string token)
        {
            Subscriber subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Token == token);
            if (subscriber == null)
                return false;

            if (!context.User.IsInRole(Constants.SubscriberRole) && !context.User.IsInRole(Constants.AdminRole))
            {
                var claims = new[] { new Claim("user", subscriber.Token), new Claim("role", Constants.SubscriberRole) };
                await context.SignInAsync(Constants.AuthCookieScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")));
            }

            return true;
        }
    }
}
