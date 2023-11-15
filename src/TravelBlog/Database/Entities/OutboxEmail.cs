namespace TravelBlog.Database.Entities;

public class OutboxEmail
{
    public int Id { get; set; }

    public int? BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; }

    public required string EmailAddress { get; set; }
    public required byte[] Content { get; set; }
}
