using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrafficTimetable.Infrastructure.Migrations
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

            migrationBuilder.CreateTable(
                name: "ClientStates",
                columns: table => new
                {
                    ClientId = table.Column<string>(nullable: false),
                    SessionId = table.Column<string>(nullable: true),
                    ClientStatus = table.Column<int>(nullable: false),
                    WaitingToContinue = table.Column<bool>(nullable: false),
                    BufferDirection = table.Column<string>(nullable: true),
                    BufferStopName = table.Column<string>(nullable: true),
                    BufferRouteName = table.Column<string>(nullable: true),
                    BufferTagName = table.Column<string>(nullable: true),
                    BufferTransportType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientStates", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_ClientStates_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_ClientTags_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientTags_Stops_StopId",
                        column: x => x.StopId,
                        principalTable: "Stops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Id",
                table: "Clients",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClientStates_ClientId",
                table: "ClientStates",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientTags_StopId",
                table: "ClientTags",
                column: "StopId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientTags_ClientId_TagName",
                table: "ClientTags",
                columns: new[] { "ClientId", "TagName" });

            migrationBuilder.CreateIndex(
                name: "IX_Stops_Id",
                table: "Stops",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientStates");

            migrationBuilder.DropTable(
                name: "ClientTags");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Stops");
        }
    }
}
