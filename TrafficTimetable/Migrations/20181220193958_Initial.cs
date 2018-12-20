using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrafficTimetable.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientStates",
                columns: table => new
                {
                    ClientId = table.Column<string>(nullable: false),
                    SessionId = table.Column<string>(nullable: false),
                    IsAddName = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsAddStop = table.Column<bool>(nullable: false),
                    IsAddRoute = table.Column<bool>(nullable: false),
                    IsAddTag = table.Column<bool>(nullable: false),
                    IsChoosingDirection = table.Column<bool>(nullable: false),
                    BufferDirection = table.Column<string>(nullable: true),
                    BufferStopName = table.Column<string>(nullable: true),
                    BufferRouteName = table.Column<string>(nullable: true),
                    BufferTagName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientStates", x => new { x.ClientId, x.SessionId });
                });

            migrationBuilder.CreateTable(
                name: "ClientTags",
                columns: table => new
                {
                    ClientId = table.Column<string>(nullable: false),
                    TagName = table.Column<string>(nullable: false),
                    StopId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientTags", x => new { x.ClientId, x.TagName });
                });

            migrationBuilder.CreateTable(
                name: "Stops",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Routes = table.Column<List<string>>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stops", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "ClientStates");

            migrationBuilder.DropTable(
                name: "ClientTags");

            migrationBuilder.DropTable(
                name: "Stops");
        }
    }
}
