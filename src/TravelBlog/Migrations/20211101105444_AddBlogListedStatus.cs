using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelBlog.Migrations
{
    public partial class AddBlogListedStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Listed",
                table: "BlogPosts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE BlogPosts SET Listed = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Listed",
                table: "BlogPosts");
        }
    }
}
