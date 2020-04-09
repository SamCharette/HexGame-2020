using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class AddPlayerInfoStrings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Player1Info",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Player2Info",
                table: "Games",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Player1Info",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Player2Info",
                table: "Games");
        }
    }
}
