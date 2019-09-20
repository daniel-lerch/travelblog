using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace TravelBlog.Migrations
{
    public partial class FixForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostReads");

            migrationBuilder.CreateTable(
                name: "PostReads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PostId = table.Column<int>(nullable: false),
                    SubscriberId = table.Column<int>(nullable: false),
                    AccessTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReads_BlogPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReads_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostReads_PostId",
                table: "PostReads",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReads_SubscriberId",
                table: "PostReads",
                column: "SubscriberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostReads");

            migrationBuilder.CreateTable(
                name: "PostReads",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PostId = table.Column<int>(nullable: false),
                    SubscriberId = table.Column<int>(nullable: false),
                    AccessTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReads_BlogPosts_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReads_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostReads_SubscriberId",
                table: "PostReads",
                column: "SubscriberId");
        }
    }
}
