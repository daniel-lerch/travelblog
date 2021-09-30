namespace TravelBlog.Database.Entities
{
    public class MailJob
    {
        public MailJob(int id, int subscriberId, string subject, string content)
        {
            Id = id;
            SubscriberId = subscriberId;
            Subject = subject;
            Content = content;
        }

        public int Id { get; set; }

        public int SubscriberId { get; set; }
        public Subscriber? Subscriber { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
