using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;
using TravelBlog.Configuration;

namespace TravelBlog.Services;

/// <summary>
/// Background service to send mails and handle rate limits and retry attempts.
/// </summary>
public class MailingService : IAsyncDisposable, IDisposable
{
    private readonly IOptions<MailingOptions> options;
    private readonly ILogger<MailingService> logger;
    private SmtpClient? client;

    public MailingService(IOptions<MailingOptions> options, ILogger<MailingService> logger)
    {
        this.options = options;
        this.logger = logger;
    }

    public async Task<bool> SendMailAsync(string address, string name, string subject, string content)
    {
        if (disposedValue) throw new ObjectDisposedException(nameof(MailingService));

        MailingOptions options = this.options.Value;

        if (!options.EnableMailing)
        {
            logger.LogWarning($"Mailing is disabled. {name} <{address}> will not receive an email.");
            return true;
        }

        var sender = new MailboxAddress(options.SenderName, options.SenderAddress);
        var author = string.IsNullOrEmpty(options.AuthorAddress) ? sender : new MailboxAddress(options.AuthorName, options.AuthorAddress);
        var message = new MimeMessage();
        message.From.Add(sender);
        message.ReplyTo.Add(author);
        message.To.Add(new MailboxAddress(name, address));
        message.Subject = subject;
        message.Body = new TextPart("plain")
        {
            Text = content
        };

        try
        {
            if (client == null)
            {
                client = new SmtpClient();
                await client.ConnectAsync(options.SmtpHost, options.SmtpPort, options.UseSsl);
                await client.AuthenticateAsync(options.SmtpUsername, options.SmtpPassword);
            }

            // We have to handle the case of an SMTP rate limit when multiple customers are supposed to receive a mail at the same time
            // Strato for reference only allows you to send 50 emails without delay (September 2021)
            //
            // RFC 821 defines common status code as 450 mailbox unavailable (busy or blocked for policy reasons)
            // RFC 3463 defines enhanced status code as 4.7.X for persistent transient failures caused by security or policy status
            await client.SendAsync(message);
            logger.LogInformation($"Successfully sent email to {address}.");
            return true;
        }
        catch (SmtpCommandException ex) when (ex.StatusCode == SmtpStatusCode.ServiceNotAvailable)
        {
            logger.LogInformation("SMTP session closed by server. This is most likely just a normal idle timeout.");
            return false;
        }
        catch (SmtpCommandException ex) when (ex.StatusCode == SmtpStatusCode.MailboxBusy)
        {
            logger.LogInformation("Mailbox busy. This is most likely caused by a temporary rate limit.");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to send mail to {name} <{address}>.");
            if (client != null)
            {
                await client.DisconnectAsync(quit: true);
                client.Dispose();
                client = null;
            }
            return true;
        }
    }

    #region IAsyncDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    public async ValueTask DisposeAsync()
    {
        if (!disposedValue)
        {
            if (client != null)
            {
                await client.DisconnectAsync(quit: true);
                client.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        if (!disposedValue)
        {
            if (client != null)
            {
                client.Disconnect(quit: true);
                client.Dispose();
            }

            disposedValue = true;
        }
    }
    #endregion
}
