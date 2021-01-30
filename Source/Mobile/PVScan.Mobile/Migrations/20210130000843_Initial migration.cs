using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Mobile.Migrations
{
    public partial class Initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Barcodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Format = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    ScanLocation_Latitude = table.Column<double>(nullable: true),
                    ScanLocation_Longitude = table.Column<double>(nullable: true),
                    ServerSynced = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barcodes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Barcodes");
        }
    }
}
