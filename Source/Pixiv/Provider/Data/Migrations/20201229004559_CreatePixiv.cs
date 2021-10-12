using Microsoft.EntityFrameworkCore.Migrations;

namespace EHunter.Pixiv.Data.Migrations
{
    public partial class CreatePixiv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PixivPendingDownloads",
                columns: table => new
                {
                    ArtworkId = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PixivPendingDownloads", x => x.ArtworkId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PixivPendingDownloads");
        }
    }
}
