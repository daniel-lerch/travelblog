using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Utilities;

namespace TravelBlog.Services;

public class EmailDeliveryService
{
    private readonly DatabaseContext database;
    private readonly JobQueue<EmailDeliveryJobController> jobQueue;

    public EmailDeliveryService(DatabaseContext database, JobQueue<EmailDeliveryJobController> jobQueue)
    {
        this.database = database;
        this.jobQueue = jobQueue;
    }

    public async ValueTask<bool> Enqueue(string emailAddress, MimeMessage mimeMessage, int? blogPostId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(emailAddress)) throw new ArgumentException("The recipient address must not be null or empty", nameof(emailAddress));

        if (blogPostId.HasValue && await database.OutboxEmails
                .AnyAsync(email => email.EmailAddress == emailAddress && email.BlogPostId == blogPostId, cancellationToken))
            return false;

        byte[] content;

        using (MemoryStream memoryStream = new())
        {
            mimeMessage.WriteTo(memoryStream, CancellationToken.None);
            content = memoryStream.ToArray();
        }

        database.OutboxEmails.Add(new()
        {
            EmailAddress = emailAddress,
            Content = content,
            BlogPostId = blogPostId
        });
        await database.SaveChangesAsync(cancellationToken);
        jobQueue.EnsureRunning();
        return true;
    }

    public async ValueTask Enqueue(IEnumerable<(string emailAddress, MimeMessage mimeMessage)> messages, int? blogPostId)
    {
        database.OutboxEmails.AddRange(messages.Select(message =>
        {
            byte[] content;

            using (MemoryStream memoryStream = new())
            {
                message.mimeMessage.WriteTo(memoryStream, CancellationToken.None);
                content = memoryStream.ToArray();
            }

            return new OutboxEmail()
            {
                EmailAddress = message.emailAddress,
                Content = content,
                BlogPostId = blogPostId,
            };
        }));

        await database.SaveChangesAsync();
        jobQueue.EnsureRunning();
    }
}
