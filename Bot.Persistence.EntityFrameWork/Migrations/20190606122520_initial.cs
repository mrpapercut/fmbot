using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Bot.Persistence.EntityFrameWork.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "servers",
                columns: table => new
                {
                    id = table.Column<decimal>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    prefix = table.Column<string>(nullable: true),
                    totalmembers = table.Column<int>(nullable: false),
                    joindate = table.Column<DateTime>(nullable: false),
                    active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<decimal>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    commandused = table.Column<DateTime>(nullable: false),
                    timestimedout = table.Column<int>(nullable: false),
                    spamwarning = table.Column<int>(nullable: false),
                    commandspam = table.Column<int>(nullable: false),
                    featured = table.Column<bool>(nullable: true),
                    lastfmusername = table.Column<string>(nullable: false),
                    defaulttimespan = table.Column<int>(nullable: false),
                    usertype = table.Column<int>(nullable: false),
                    defaultfmtype = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "requests",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    timestamp = table.Column<DateTime>(nullable: false),
                    command = table.Column<string>(nullable: false),
                    errormessage = table.Column<string>(nullable: true),
                    issuccessfull = table.Column<bool>(nullable: false),
                    runtime = table.Column<long>(nullable: false),
                    serverid = table.Column<decimal>(nullable: true),
                    userid = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => new { x.timestamp, x.id });
                    table.ForeignKey(
                        name: "FK_requests_servers_serverid",
                        column: x => x.serverid,
                        principalTable: "servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_requests_users_userid",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_requests_serverid",
                table: "requests",
                column: "serverid");

            migrationBuilder.CreateIndex(
                name: "IX_requests_userid",
                table: "requests",
                column: "userid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "requests");

            migrationBuilder.DropTable(
                name: "servers");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
