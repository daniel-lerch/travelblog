using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelBlog.Configuration;
using TravelBlog.Database.Entities;

namespace TravelBlog.Database;

public class DatabaseContext : DbContext
{
    private readonly IOptions<DatabaseOptions> options;

    public DbSet<Page> Pages => Set<Page>();
    public DbSet<Subscriber> Subscribers => Set<Subscriber>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<PostRead> PostReads => Set<PostRead>();
    public DbSet<OutboxEmail> OutboxEmails => Set<OutboxEmail>();
    public DbSet<SentEmail> SentEmails => Set<SentEmail>();

    public DatabaseContext(IOptions<DatabaseOptions> options)
    {
        this.options = options;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite(options.Value.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var page = modelBuilder.Entity<Page>();
        page.HasKey(p => p.Name);

        var subscriber = modelBuilder.Entity<Subscriber>();
        subscriber.HasKey(s => s.Id);
        subscriber.HasIndex(s => s.MailAddress).IsUnique();
        subscriber.HasIndex(s => s.Token).IsUnique();

        var blogPost = modelBuilder.Entity<BlogPost>();
        blogPost.HasKey(p => p.Id);

        var postRead = modelBuilder.Entity<PostRead>();
        postRead.HasKey(r => r.Id);
        postRead.HasOne(r => r.Post).WithMany(p => p!.Reads).HasForeignKey(r => r.PostId);
        postRead.HasOne(r => r.Subscriber).WithMany(s => s!.Reads).HasForeignKey(r => r.SubscriberId);

        var outboxEmail = modelBuilder.Entity<OutboxEmail>();
        outboxEmail.HasKey(e => e.Id);
        outboxEmail.HasOne(e => e.BlogPost).WithMany().HasForeignKey(e => e.BlogPostId);

        var sentEmail = modelBuilder.Entity<SentEmail>();
        sentEmail.HasKey(e => e.Id);
        sentEmail.HasOne(e => e.BlogPost).WithMany().HasForeignKey(e => e.BlogPostId);
        sentEmail.HasIndex(e => e.DeliveryTime);
        sentEmail.Property(e => e.Id).ValueGeneratedNever();
    }
}
