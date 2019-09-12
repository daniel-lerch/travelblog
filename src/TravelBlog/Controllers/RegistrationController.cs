using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Models;

namespace TravelBlog.Controllers
{
    [Route("~/register")]
    public class RegistrationController : Controller
    {
        private DatabaseContext context;

        public RegistrationController(DatabaseContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index", new RegistrationViewModel("", "", ""));
        }

        [HttpPost]
        public IActionResult Post(string mailAddress, string givenName, string familyName)
        {
            //context.Users.Add(new User { MailAddress = mailAddress, GivenName = givenName, FamilyName = familyName });
            return View("Index", new RegistrationViewModel(mailAddress, givenName, familyName, "Fehler: Keine Datenbankverbindung"));
        }
    }
}