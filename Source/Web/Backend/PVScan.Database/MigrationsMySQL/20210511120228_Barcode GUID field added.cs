using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Database.MigrationsMySQL
{
    public partial class BarcodeGUIDfieldadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GUID",
                table: "Barcodes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GUID",
                table: "Barcodes");
        }
    }
}
