using Microsoft.EntityFrameworkCore.Migrations;

namespace PVScan.Database.Migrations
{
    public partial class AddUserInfos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarcodesScanned = table.Column<int>(type: "int", nullable: false),
                    BarcodeFormatsScanned = table.Column<int>(type: "int", nullable: false),
                    VKLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IGLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInfos");
        }
    }
}
