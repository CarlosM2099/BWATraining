using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazingPizza.Server.Migrations
{
    public partial class AddNotifacationsI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Auth",
                table: "NotificationSubscriptions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auth",
                table: "NotificationSubscriptions");
        }
    }
}
