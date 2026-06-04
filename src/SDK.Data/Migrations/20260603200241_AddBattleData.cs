using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDK.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBattleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Abilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    Power = table.Column<int>(type: "INTEGER", nullable: true),
                    Accuracy = table.Column<int>(type: "INTEGER", nullable: false),
                    PP = table.Column<int>(type: "INTEGER", nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Moves_PokemonTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "PokemonTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Learnsets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpeciesId = table.Column<int>(type: "INTEGER", nullable: false),
                    MoveId = table.Column<int>(type: "INTEGER", nullable: false),
                    LearnLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Learnsets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Learnsets_Moves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Moves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Learnsets_PokemonSpecies_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "PokemonSpecies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_Identifier",
                table: "Abilities",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Learnsets_MoveId",
                table: "Learnsets",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_Learnsets_SpeciesId_Generation",
                table: "Learnsets",
                columns: new[] { "SpeciesId", "Generation" });

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Generation",
                table: "Moves",
                column: "Generation");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_TypeId",
                table: "Moves",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Abilities");

            migrationBuilder.DropTable(
                name: "Learnsets");

            migrationBuilder.DropTable(
                name: "Moves");
        }
    }
}
