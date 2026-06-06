using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDK.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSpriteAtlas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sprite_atlas_entries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssetKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    View = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AtlasPath = table.Column<string>(type: "TEXT", maxLength: 260, nullable: false),
                    X = table.Column<int>(type: "INTEGER", nullable: false),
                    Y = table.Column<int>(type: "INTEGER", nullable: false),
                    Width = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sprite_atlas_entries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sprite_atlas_entries_AssetKey",
                table: "sprite_atlas_entries",
                column: "AssetKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sprite_atlas_entries");
        }
    }
}
