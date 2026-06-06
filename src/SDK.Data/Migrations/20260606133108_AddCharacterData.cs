using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDK.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "villain_groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_villain_groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "villain_members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    VillainGroupId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_villain_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_villain_members_characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_villain_members_villain_groups_VillainGroupId",
                        column: x => x.VillainGroupId,
                        principalTable: "villain_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_characters_Identifier",
                table: "characters",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_villain_groups_Identifier",
                table: "villain_groups",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_villain_members_CharacterId",
                table: "villain_members",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_villain_members_VillainGroupId",
                table: "villain_members",
                column: "VillainGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "villain_members");

            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.DropTable(
                name: "villain_groups");
        }
    }
}
