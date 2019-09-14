using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBlog.Configuration;
using TravelBlog.Database.Entities;

namespace TravelBlog.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly IOptions<DatabaseOptions> options;

        public DbSet<Subscriber> Subscribers { get; set; }

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
            subscriber.HasAlternateKey(s => s.MailAddress);
            subscriber.HasAlternateKey(s => s.Token);
            subscriber.Property(s => s.MailAddress).IsRequired();
            subscriber.Property(s => s.GivenName).IsRequired();
            subscriber.Property(s => s.FamilyName).IsRequired();
        }
    }
}
