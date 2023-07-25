using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using Wiry.Base32;

namespace TravelBlog.Controllers;

[ApiController]
public class SubscriberApiController : ControllerBase
{
    private readonly ILogger<SubscriberApiController> logger;
    private readonly DatabaseContext database;

    public SubscriberApiController(ILogger<SubscriberApiController> logger, DatabaseContext database)
    {
        this.logger = logger;
        this.database = database;
    }

    [HttpPost("~/api/subscribe")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
    {
        if (!ModelState.IsValid) return StatusCode(StatusCodes.Status400BadRequest);

        database.Subscribers.Add(new(request.MailAddress, request.GivenName, request.FamilyName, RandomToken()));

        try
        {
            await database.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            logger.LogInformation("A user tried to register with {Address} which is alreay registered.", request.MailAddress);
            return StatusCode(StatusCodes.Status409Conflict);
        }
        return StatusCode(StatusCodes.Status204NoContent);
    }

    [HttpGet("~/api/profile")]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Profile([FromQuery] string token)
    {
		Subscriber? subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Token == token);
		if (subscriber is null)
			return StatusCode(StatusCodes.Status404NotFound);

        if (subscriber.MailAddress == null)
            throw new InvalidDataException(
                $"{nameof(subscriber.MailAddress)} must not be null when {nameof(subscriber.Token)} is not null.");

		return new JsonResult(new ProfileResponse(subscriber.MailAddress, subscriber.GivenName, subscriber.FamilyName));
    }

    [HttpPost("~/api/unsubscribe")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unsubscribe([FromQuery] string token)
    {
        Subscriber? subscriber = await database.Subscribers.SingleOrDefaultAsync(s => s.Token == token);
        if (subscriber is null)
            return StatusCode(StatusCodes.Status404NotFound);

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
        return StatusCode(StatusCodes.Status204NoContent);
    }

    public record SubscribeRequest(string MailAddress, string GivenName, string FamilyName, string Comment);

    public record ProfileResponse(string MailAddress, string GivenName, string FamilyName);

    private static string RandomToken()
    {
        byte[] buffer = new byte[20];
        RandomNumberGenerator.Fill(buffer);
        return Base32Encoding.Standard.GetString(buffer);
    }
}
