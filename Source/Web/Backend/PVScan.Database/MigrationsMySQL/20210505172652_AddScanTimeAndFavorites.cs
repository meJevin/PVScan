using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Database.MigrationsMySQL
{
    public partial class AddScanTimeAndFavorites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Favorite",
                table: "Barcodes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScanTime",
                table: "Barcodes",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Favorite",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "ScanTime",
                table: "Barcodes");
        }
    }
}
