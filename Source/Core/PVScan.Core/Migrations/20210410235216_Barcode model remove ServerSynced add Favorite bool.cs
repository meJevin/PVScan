using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Core.Migrations
{
    public partial class BarcodemodelremoveServerSyncedaddFavoritebool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServerSynced",
                table: "Barcodes",
                newName: "Favorite");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Favorite",
                table: "Barcodes",
                newName: "ServerSynced");
        }
    }
}
