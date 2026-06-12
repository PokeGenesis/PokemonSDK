using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDK.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFakemonSpecies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fakemon_species",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseHp = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseAttack = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseDefense = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseSpecialAtk = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseSpecialDef = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseSpeed = table.Column<int>(type: "INTEGER", nullable: false),
                    Type1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Type2Id = table.Column<int>(type: "INTEGER", nullable: true),
                    EggGroup1 = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EggGroup2 = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsLegendary = table.Column<bool>(type: "INTEGER", nullable: false),
                    PartsManifest = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fakemon_species", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fakemon_species_PokemonTypes_Type1Id",
                        column: x => x.Type1Id,
                        principalTable: "PokemonTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fakemon_species_PokemonTypes_Type2Id",
                        column: x => x.Type2Id,
                        principalTable: "PokemonTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fakemon_species_Identifier",
                table: "fakemon_species",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fakemon_species_Type1Id",
                table: "fakemon_species",
                column: "Type1Id");

            migrationBuilder.CreateIndex(
                name: "IX_fakemon_species_Type2Id",
                table: "fakemon_species",
                column: "Type2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fakemon_species");
        }
    }
}
