using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBlog.Configuration;

namespace TravelBlog.Services
{
    public class MailingService : IAsyncDisposable
    {
        private readonly IOptions<MailingOptions> options;
        private readonly ILogger<MailingService> logger;
        private SmtpClient? client;

        public MailingService(IOptions<MailingOptions> options, ILogger<MailingService> logger)
        {
            this.options = options;
            this.logger = logger;
        }

        public async Task SendMailAsync(string name, string address, string subject, string content, string unsubscribeLink)
        {
            MailingOptions options = this.options.Value;

            if (!options.EnableMailing)
            {
                logger.LogWarning($"Mailing is disabled. {name}<{address}> will not receive any confirmation or notification.");
                return;
            }

            var sender = new MailboxAddress(options.SenderName, options.SenderAddress);
            var author = string.IsNullOrEmpty(options.AuthorAddress) ? sender : new MailboxAddress(options.AuthorName, options.AuthorAddress);
            var message = new MimeMessage();
            message.From.Add(author);
            message.Sender = sender;
            message.To.Add(new MailboxAddress(name, address));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = content + "\r\n\r\nWenn du keine E-Mails mehr von diesem Blog erhalten möchtest, klicke einfach auf folgenden Link: " + unsubscribeLink
            };

            if (client == null)
            {
                client = new SmtpClient();
                await client.ConnectAsync(options.SmtpHost, options.SmtpPort, options.UseSsl);
                await client.AuthenticateAsync(options.SmtpUsername, options.SmtpPassword);
            }

            await client.SendAsync(message);
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
        #endregion
    }
}
