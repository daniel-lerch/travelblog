using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelBlog.Database;
using TravelBlog.Database.Entities;

namespace TravelBlog.Services.LightJobManager;

public class MailJobContext : IJobContext<MailJob>
{
    private readonly DatabaseContext database;
    private readonly MailingService mailing;

    public MailJobContext(DatabaseContext database, MailingService mailing)
    {
        this.database = database;
        this.mailing = mailing;
    }

    public Task Add(MailJob data)
    {
        database.MailJobs.Add(data);
        return database.SaveChangesAsync();
    }

    public Task AddRange(IEnumerable<MailJob> data)
    {
        database.MailJobs.AddRange(data);
        return database.SaveChangesAsync();
    }

    public Task Remove(MailJob data)
    {
        database.MailJobs.Remove(data);
        return database.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<MailJob>> GetJobs()
    {
        return await database.MailJobs.Include(j => j.Subscriber).ToListAsync();
    }

    public Task<bool> Execute(MailJob data)
    {
        if (data.Subscriber!.DeletionTime == default)
        {
            return mailing.SendMailAsync(data.Subscriber!.MailAddress!, data.Subscriber.GetName(), data.Subject, data.Content);
        }
        else
        {
            // A mail job might be queued when a subscriber is still registered
            // but the subscriber might unsubscribe before the job is fetched and executed.
            return Task.FromResult(true);
        }
    }
}
