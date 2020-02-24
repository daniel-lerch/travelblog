﻿using Microsoft.AspNetCore.Authentication;
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

        public Task SignInAsync(HttpContext context, string user, string role)
        {
            var claims = new[] { new Claim("user", user), new Claim("role", role) };
            var properties = new AuthenticationProperties { IsPersistent = true };
            return context.SignInAsync(Constants.AuthCookieScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "user", "role")), properties);
        }

        public Task SignOutAsync(HttpContext context)
        {
            return context.SignOutAsync(Constants.AuthCookieScheme);
        }

        public async Task<bool> SignInAsync(HttpContext context, string token)
        {
            Subscriber subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Token == token);
            if (subscriber == null)
                return false;

            if (!context.User.IsInRole(Constants.SubscriberRole) && !context.User.IsInRole(Constants.AdminRole))
            {
                await SignInAsync(context, token, Constants.SubscriberRole);
            }

            return true;
        }
    }
}
