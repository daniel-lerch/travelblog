using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBlog.Database.Entities;

namespace TravelBlog.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DatabaseContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite("");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var user = modelBuilder.Entity<User>();
            user.HasKey(u => u.Id);
            user.HasAlternateKey(u => u.MailAddress);
            user.Property(u => u.MailAddress).IsRequired();
            user.Property(u => u.GivenName).IsRequired();
            user.Property(u => u.FamilyName).IsRequired();
        }
    }
}
