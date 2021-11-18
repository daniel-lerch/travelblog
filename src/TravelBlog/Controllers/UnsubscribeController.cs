using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Models;

namespace TravelBlog.Controllers
{
    [Route("~/unsubscribe")]
    public class UnsubscribeController : Controller
    {
        private readonly DatabaseContext database;

        public UnsubscribeController(DatabaseContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string token)
        {
            Subscriber? subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Token == token);
            if (subscriber is null)
                return View("InvalidToken");

            return View("Pending", new UnsubscribeViewModel(subscriber));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] string token)
        {
            Subscriber? subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Token == token);
            if (subscriber is null)
                return View("InvalidToken");

            if (subscriber.ConfirmationTime == default)
            {
                database.Subscribers.Remove(subscriber);
            }
            else
            {
                subscriber.MailAddress = null;
                subscriber.DeletionTime = DateTime.Now;
                subscriber.Token = null;
            }
            await database.SaveChangesAsync();
            return View("Success", new UnsubscribeViewModel(subscriber));
        }
    }
}
