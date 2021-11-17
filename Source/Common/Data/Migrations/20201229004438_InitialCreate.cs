using Microsoft.EntityFrameworkCore.Migrations;

namespace EHunter.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublishedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FavoritedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Url = table.Column<string>(type: "varchar(450)", nullable: true),
                    Provider = table.Column<string>(type: "varchar(32)", nullable: true),
                    Identifier = table.Column<int>(type: "int", nullable: true),
                    AdditionalIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DetailText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TagConverts",
                columns: table => new
                {
                    TagScopeName = table.Column<string>(type: "varchar(32)", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConvertedTagScopeName = table.Column<string>(type: "varchar(32)", nullable: true),
                    ConvertedTagName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagConverts", x => new { x.TagScopeName, x.TagName });
                });

            migrationBuilder.CreateTable(
                name: "TagImplies",
                columns: table => new
                {
                    TagScopeName = table.Column<string>(type: "varchar(32)", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImpliedTagScopeName = table.Column<string>(type: "varchar(32)", nullable: false),
                    ImpliedTagName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagImplies", x => new { x.TagScopeName, x.TagName, x.ImpliedTagScopeName, x.ImpliedTagName });
                });

            migrationBuilder.CreateTable(
                name: "Galleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Galleries_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StoragePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "varchar(450)", nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    PostOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Images_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GalleryTags",
                columns: table => new
                {
                    GalleryId = table.Column<int>(type: "int", nullable: false),
                    TagScopeName = table.Column<string>(type: "varchar(32)", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalleryTags", x => new { x.GalleryId, x.TagScopeName, x.TagName });
                    table.ForeignKey(
                        name: "FK_GalleryTags_Galleries_GalleryId",
                        column: x => x.GalleryId,
                        principalTable: "Galleries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageTags",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false),
                    TagScopeName = table.Column<string>(type: "varchar(32)", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageTags", x => new { x.ImageId, x.TagScopeName, x.TagName });
                    table.ForeignKey(
                        name: "FK_ImageTags_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_PostId",
                table: "Galleries",
                column: "PostId",
                unique: true,
                filter: "[PostId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Images_PostId_PostOrderId",
                table: "Images",
                columns: new[] { "PostId", "PostOrderId" },
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Provider_Identifier",
                table: "Posts",
                columns: new[] { "Provider", "Identifier" },
                unique: true,
                filter: "[Provider] IS NOT NULL AND [Identifier] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GalleryTags");

            migrationBuilder.DropTable(
                name: "ImageTags");

            migrationBuilder.DropTable(
                name: "TagConverts");

            migrationBuilder.DropTable(
                name: "TagImplies");

            migrationBuilder.DropTable(
                name: "Galleries");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
