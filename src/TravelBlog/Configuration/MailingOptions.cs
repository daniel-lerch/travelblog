using System;
using System.ComponentModel.DataAnnotations;

namespace TravelBlog.Configuration;

public class MailingOptions
{
    public bool EnableMailing { get; set; }
    [Required] public string? SenderName { get; set; }
    [Required, EmailAddress] public string? SenderAddress { get; set; }
    [Required] public string? AuthorName { get; set; }
    [Required, EmailAddress] public string? AuthorAddress { get; set; }

    [Required] public string? SmtpUsername { get; set; }
    [Required] public string? SmtpPassword { get; set; }
    public bool UseSsl { get; set; }
    [Required] public string? SmtpHost { get; set; }
    [Range(1, 65535)] public ushort SmtpPort { get; set; }
}
