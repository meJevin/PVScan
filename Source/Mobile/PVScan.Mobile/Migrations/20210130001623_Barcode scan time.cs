using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Mobile.Migrations
{
    public partial class Barcodescantime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ScanTime",
                table: "Barcodes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScanTime",
                table: "Barcodes");
        }
    }
}
