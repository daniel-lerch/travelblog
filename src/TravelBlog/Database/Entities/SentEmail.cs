using System;

namespace TravelBlog.Database.Entities;

public class SentEmail
{
    public int Id { get; set; }

    public int? BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; }

    public required string EmailAddress { get; set; }
    public required int ContentSize { get; set; }
    public string? ErrorMessage { get; set; }
    public required DateTime DeliveryTime { get; set; }
}
