using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Core.Migrations
{
    public partial class BarcodeGUIDadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GUID",
                table: "Barcodes",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GUID",
                table: "Barcodes");
        }
    }
}
