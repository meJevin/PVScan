using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Database.Migrations
{
    public partial class AddUserInfosExperience : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Experience",
                table: "UserInfos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "UserInfos",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Experience",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "UserInfos");
        }
    }
}
