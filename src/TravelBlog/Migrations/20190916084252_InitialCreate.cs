using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelBlog.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MailAddress = table.Column<string>(nullable: false),
                    GivenName = table.Column<string>(nullable: false),
                    FamilyName = table.Column<string>(nullable: false),
                    ConfirmationTime = table.Column<DateTime>(nullable: false),
                    Token = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.Id);
                    table.UniqueConstraint("AK_Subscribers_MailAddress", x => x.MailAddress);
                    table.UniqueConstraint("AK_Subscribers_Token", x => x.Token);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscribers");
        }
    }
}
