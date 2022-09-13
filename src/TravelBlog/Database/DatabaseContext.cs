using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelBlog.Configuration;
using TravelBlog.Database.Entities;

namespace TravelBlog.Database;

public class DatabaseContext : DbContext
{
    private readonly IOptions<DatabaseOptions> options;

    public DbSet<Subscriber> Subscribers => Set<Subscriber>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<PostRead> PostReads => Set<PostRead>();
    public DbSet<MailJob> MailJobs => Set<MailJob>();

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

        var mailJob = modelBuilder.Entity<MailJob>();
        mailJob.HasKey(t => t.Id);
        mailJob.HasOne(t => t.Subscriber).WithMany().HasForeignKey(t => t.SubscriberId);
    }
}
