using System;
using System.Collections.Generic;
using System.Linq;
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

    public record JsonSubscriber(
        int Id,
        string? MailAddress,
        string GivenName,
        string FamilyName,
        DateTime? ConfirmationTime,
        DateTime? DeletionTime)
    {
        public JsonSubscriber(Subscriber subscriber)
            : this(
                subscriber.Id,
                subscriber.MailAddress,
                subscriber.GivenName,
                subscriber.FamilyName,
                subscriber.ConfirmationTime.NullIfDefault(),
                subscriber.DeletionTime.NullIfDefault())
        { }
    }
}
