using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDK.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExpSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseExpYield",
                table: "PokemonSpecies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 64);

            migrationBuilder.AddColumn<int>(
                name: "GrowthRate",
                table: "PokemonSpecies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseExpYield",
                table: "PokemonSpecies");

            migrationBuilder.DropColumn(
                name: "GrowthRate",
                table: "PokemonSpecies");
        }
    }
}
