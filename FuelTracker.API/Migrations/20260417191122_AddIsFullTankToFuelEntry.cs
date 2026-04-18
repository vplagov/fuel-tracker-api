using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFullTankToFuelEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFullTank",
                table: "FuelEntries",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFullTank",
                table: "FuelEntries");
        }
    }
}
