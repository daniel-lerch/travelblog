using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelBlog.Migrations
{
    public partial class PreventCascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys=OFF", suppressTransaction: true);

            migrationBuilder.CreateTable(
                name: "MigratedSubscribers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MailAddress = table.Column<string>(nullable: true),
                    GivenName = table.Column<string>(nullable: false),
                    FamilyName = table.Column<string>(nullable: false),
                    ConfirmationTime = table.Column<DateTime>(nullable: false),
                    DeletionTime = table.Column<DateTime>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.Id);
                });

            // Copy data
            migrationBuilder.Sql("INSERT INTO MigratedSubscribers (Id, MailAddress, GivenName, FamilyName, ConfirmationTime, DeletionTime, Token) " +
                "SELECT Id, MailAddress, GivenName, FamilyName, ConfirmationTime, \"0001-01-01 00:00:00\", Token FROM Subscribers");

            migrationBuilder.DropTable("Subscribers");

            migrationBuilder.RenameTable("MigratedSubscribers", newName: "Subscribers");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_MailAddress",
                table: "Subscribers",
                column: "MailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_Token",
                table: "Subscribers",
                column: "Token",
                unique: true);

            migrationBuilder.Sql("PRAGMA foreign_keys=ON", suppressTransaction: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys=OFF", suppressTransaction: true);

            migrationBuilder.DropIndex(
                name: "IX_Subscribers_MailAddress",
                table: "Subscribers");

            migrationBuilder.DropIndex(
                name: "IX_Subscribers_Token",
                table: "Subscribers");

            migrationBuilder.CreateTable(
                name: "MigratedSubscribers",
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

            // Copy data and delete archived subscribers
            migrationBuilder.Sql("INSERT INTO MigratedSubscribers (Id, MailAddress, GivenName, FamilyName, ConfirmationTime, Token) " +
                "SELECT Id, MailAddress, GivenName, FamilyName, ConfirmationTime, Token FROM Subscribers " +
                "WHERE DeletionTime <> \"0001-01-01 00:00:00\"");

            migrationBuilder.DropTable("Subscribers");

            migrationBuilder.RenameTable("MigratedSubscribers", newName: "Subscribers");

            migrationBuilder.Sql("PRAGMA foreign_keys=ON", suppressTransaction: true);
        }
    }
}
