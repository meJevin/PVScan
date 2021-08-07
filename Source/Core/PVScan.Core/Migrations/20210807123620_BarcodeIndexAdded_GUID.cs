using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Core.Migrations
{
    public partial class BarcodeIndexAdded_GUID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_GUID",
                table: "Barcodes",
                column: "GUID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Barcodes_GUID",
                table: "Barcodes");
        }
    }
}
