﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TravelBlog.Database;

namespace TravelBlog.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20190919140816_AddBlogsAndReads")]
    partial class AddBlogsAndReads
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("TravelBlog.Database.Entities.BlogPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .IsRequired();

                    b.Property<DateTime>("ModifyTime");

                    b.Property<DateTime>("PublishTime");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("BlogPosts");
                });

            modelBuilder.Entity("TravelBlog.Database.Entities.PostRead", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AccessTime");

                    b.Property<int>("PostId");

                    b.Property<int>("SubscriberId");

                    b.HasKey("Id");

                    b.HasIndex("SubscriberId");

                    b.ToTable("PostReads");
                });

            modelBuilder.Entity("TravelBlog.Database.Entities.Subscriber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ConfirmationTime");

                    b.Property<string>("FamilyName")
                        .IsRequired();

                    b.Property<string>("GivenName")
                        .IsRequired();

                    b.Property<string>("MailAddress")
                        .IsRequired();

                    b.Property<string>("Token")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("MailAddress");

                    b.HasAlternateKey("Token");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("TravelBlog.Database.Entities.PostRead", b =>
                {
                    b.HasOne("TravelBlog.Database.Entities.BlogPost", "Post")
                        .WithMany("Reads")
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TravelBlog.Database.Entities.Subscriber", "Subscriber")
                        .WithMany("Reads")
                        .HasForeignKey("SubscriberId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}