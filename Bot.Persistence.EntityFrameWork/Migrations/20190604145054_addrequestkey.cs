using Microsoft.EntityFrameworkCore.Migrations;

namespace Bot.Persistence.EntityFrameWork.Migrations
{
    public partial class addrequestkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_requests_timestamp",
                table: "requests",
                column: "timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_requests_timestamp",
                table: "requests");
        }
    }
}
