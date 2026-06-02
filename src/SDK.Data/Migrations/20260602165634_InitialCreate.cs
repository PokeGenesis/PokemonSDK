using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDK.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PokemonBaseStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FormId = table.Column<int>(type: "INTEGER", nullable: false),
                    Hp = table.Column<int>(type: "INTEGER", nullable: false),
                    Attack = table.Column<int>(type: "INTEGER", nullable: false),
                    Defense = table.Column<int>(type: "INTEGER", nullable: false),
                    SpecialAttack = table.Column<int>(type: "INTEGER", nullable: false),
                    SpecialDefense = table.Column<int>(type: "INTEGER", nullable: false),
                    Speed = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonBaseStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpeciesId = table.Column<int>(type: "INTEGER", nullable: false),
                    FormKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    AssetKey = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonSpecies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginRegion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Type1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Type2Id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonSpecies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Locale = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Field = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeEffectiveness",
                columns: table => new
                {
                    AttackerTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    DefenderTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Generation = table.Column<int>(type: "INTEGER", nullable: false),
                    DamageFactor = table.Column<decimal>(type: "TEXT", precision: 4, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeEffectiveness", x => new { x.AttackerTypeId, x.DefenderTypeId, x.Generation });
                });

            migrationBuilder.CreateIndex(
                name: "IX_PokemonSpecies_Identifier",
                table: "PokemonSpecies",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PokemonTypes_Identifier",
                table: "PokemonTypes",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_EntityType_EntityId_Locale_Field",
                table: "Translations",
                columns: new[] { "EntityType", "EntityId", "Locale", "Field" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PokemonBaseStats");

            migrationBuilder.DropTable(
                name: "PokemonForms");

            migrationBuilder.DropTable(
                name: "PokemonSpecies");

            migrationBuilder.DropTable(
                name: "PokemonTypes");

            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "TypeEffectiveness");
        }
    }
}
