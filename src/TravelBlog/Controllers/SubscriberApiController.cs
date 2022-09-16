using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelBlog.Database;
using Wiry.Base32;

namespace TravelBlog.Controllers;

[ApiController]
public class SubscriberApiController : ControllerBase
{
    private readonly DatabaseContext database;

    public SubscriberApiController(DatabaseContext database)
    {
        this.database = database;
    }

    [HttpPost("~/api/subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
    {
        if (!ModelState.IsValid) return StatusCode(400);

        database.Subscribers.Add(new(request.MailAddress, request.GivenName, request.FamilyName, RandomToken()));

        try
        {
            await database.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            return StatusCode(409);
        }
        return StatusCode(204);
    }

    public record SubscribeRequest(string MailAddress, string GivenName, string FamilyName, string Comment);

    private string RandomToken()
    {
        byte[] buffer = new byte[20];
        RandomNumberGenerator.Fill(buffer);
        return Base32Encoding.Standard.GetString(buffer);
    }
}
