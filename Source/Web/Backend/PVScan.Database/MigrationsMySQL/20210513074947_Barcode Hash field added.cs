using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Database.MigrationsMySQL
{
    public partial class BarcodeHashfieldadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Barcodes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Barcodes");
        }
    }
}
