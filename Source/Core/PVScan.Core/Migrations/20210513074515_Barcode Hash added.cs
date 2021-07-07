using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Core.Migrations
{
    public partial class BarcodeHashadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Barcodes",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Barcodes");
        }
    }
}
