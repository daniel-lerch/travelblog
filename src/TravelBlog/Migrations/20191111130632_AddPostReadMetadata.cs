using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelBlog.Migrations
{
    public partial class AddPostReadMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "PostReads",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "PostReads",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "PostReads");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "PostReads");
        }
    }
}
