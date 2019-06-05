using Microsoft.EntityFrameworkCore.Migrations;

namespace Bot.Persistence.EntityFrameWork.Migrations
{
    public partial class fixdms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_requests_servers_serverid",
                table: "requests");

            migrationBuilder.AlterColumn<decimal>(
                name: "serverid",
                table: "requests",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AddForeignKey(
                name: "FK_requests_servers_serverid",
                table: "requests",
                column: "serverid",
                principalTable: "servers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_requests_servers_serverid",
                table: "requests");

            migrationBuilder.AlterColumn<decimal>(
                name: "serverid",
                table: "requests",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_requests_servers_serverid",
                table: "requests",
                column: "serverid",
                principalTable: "servers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
