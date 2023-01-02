using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Extensions;

namespace TravelBlog.Controllers;

[ApiController]
public class AdminApiController : ControllerBase
{
    private readonly DatabaseContext database;

    public AdminApiController(DatabaseContext database)
    {
        this.database = database;
    }

    [HttpGet("~/api/admin/subscribers")]
    //[Authorize(Roles = Constants.AdminRole)]
    public async Task<List<JsonSubscriber>> Subscribers()
    {
        var subscribers = await database.Subscribers.Select(s => new JsonSubscriber(s)).ToListAsync();
        return subscribers;
    }

    [HttpPut("~/api/admin/subscriber/{id}")]
    //[Authorize(Roles = Constants.AdminRole)]
    public async Task<IActionResult> EditSubscriber(int id, [FromBody] JsonSubscriber subscriber)
    {
        if (!ModelState.IsValid) return StatusCode(400);

        Subscriber? current = await database.Subscribers.SingleOrDefaultAsync(s => s.Id == id);
        if (current == null) return StatusCode(404);

        if (current.DeletionTime != default) return StatusCode(400);

        current.MailAddress = subscriber.MailAddress;
        current.GivenName = subscriber.GivenName;
        current.FamilyName = subscriber.FamilyName;
        current.ConfirmationTime = subscriber.ConfirmationTime ?? default;
        current.DeletionTime = subscriber.DeletionTime ?? default;
        await database.SaveChangesAsync();

        return StatusCode(204);
    }

    public class JsonSubscriber
    {
        [JsonConstructor]
        public JsonSubscriber(int id, string? mailAddress, string givenName, string familyName, DateTime? confirmationTime, DateTime? deletionTime)
        {
            Id = id;
            MailAddress = mailAddress;
            GivenName = givenName;
            FamilyName = familyName;
            ConfirmationTime = confirmationTime;
            DeletionTime = deletionTime;
        }

        public JsonSubscriber(Subscriber subscriber)
        {
            Id = subscriber.Id;
            MailAddress = subscriber.MailAddress;
            GivenName = subscriber.GivenName;
            FamilyName = subscriber.FamilyName;
            ConfirmationTime = subscriber.ConfirmationTime.NullIfDefault();
            DeletionTime = subscriber.DeletionTime.NullIfDefault();
        }

        public int Id { get; }
        public string? MailAddress { get; }
        public string GivenName { get; }
        public string FamilyName { get; }
        public DateTime? ConfirmationTime { get; }
        public DateTime? DeletionTime { get; }
    }
}
