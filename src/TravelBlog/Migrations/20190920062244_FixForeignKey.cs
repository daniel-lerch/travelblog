using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelBlog.Migrations
{
    public partial class FixForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostReads_BlogPosts_SubscriberId",
                table: "PostReads");

            migrationBuilder.CreateIndex(
                name: "IX_PostReads_PostId",
                table: "PostReads",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostReads_BlogPosts_PostId",
                table: "PostReads",
                column: "PostId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostReads_BlogPosts_PostId",
                table: "PostReads");

            migrationBuilder.DropIndex(
                name: "IX_PostReads_PostId",
                table: "PostReads");

            migrationBuilder.AddForeignKey(
                name: "FK_PostReads_BlogPosts_SubscriberId",
                table: "PostReads",
                column: "SubscriberId",
                principalTable: "BlogPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
