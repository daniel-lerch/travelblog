using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Models;
using Wiry.Base32;

namespace TravelBlog.Controllers
{
    [Route("~/register")]
    public class RegistrationController : Controller
    {
        private readonly DatabaseContext context;

        public RegistrationController(DatabaseContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return View("Index", new RegistrationViewModel("", "", ""));
        }

        [HttpPost]
        public async Task<IActionResult> Post([EmailAddress] string mailAddress, [Required] string givenName, [Required] string familyName)
        {
            if (!ModelState.IsValid)
                return View("Index", new RegistrationViewModel(mailAddress, givenName, familyName, "Deine Angaben sind unvollständig oder ungültig!"));

            context.Subscribers.Add(new Subscriber { MailAddress = mailAddress, GivenName = givenName, FamilyName = familyName, Token = RandomToken() });
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.IsUniqueConstraintViolation())
                    return View("Index", new RegistrationViewModel(mailAddress, givenName, familyName, "Diese E-Mail-Adresse ist bereits registriert."));
                else throw;
            }
            return View("Success", new RegistrationViewModel(mailAddress, givenName, familyName));
        }

        private string RandomToken()
        {
            using (var random = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[20];
                random.GetBytes(buffer);
                return Base32Encoding.Standard.GetString(buffer);
            }
        }
    }
}
