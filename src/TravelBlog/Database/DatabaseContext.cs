using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using TravelBlog.Configuration;
using TravelBlog.Database.Entities;

namespace TravelBlog.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly IOptions<DatabaseOptions> options;

        // These properties are automatically set by EF Core
        public DbSet<Subscriber> Subscribers { get; set; } = null!;
        public DbSet<BlogPost> BlogPosts { get; set; } = null!;
        public DbSet<PostRead> PostReads { get; set; } = null!;

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
            subscriber.Property(s => s.GivenName).IsRequired();
            subscriber.Property(s => s.FamilyName).IsRequired();

            var blogPost = modelBuilder.Entity<BlogPost>();
            blogPost.HasKey(p => p.Id);
            blogPost.Property(p => p.Title).IsRequired();
            blogPost.Property(p => p.Content).IsRequired();

            var postRead = modelBuilder.Entity<PostRead>();
            postRead.HasKey(r => r.Id);
            postRead.HasOne(r => r.Post).WithMany(p => p!.Reads).HasForeignKey(r => r.PostId);
            postRead.HasOne(r => r.Subscriber).WithMany(s => s!.Reads).HasForeignKey(r => r.SubscriberId);
        }
    }
}
