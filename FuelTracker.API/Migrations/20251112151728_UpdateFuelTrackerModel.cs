using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFuelTrackerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelEntries_Cars_CarId",
                table: "FuelEntries");

            migrationBuilder.DropIndex(
                name: "IX_FuelEntries_CarId",
                table: "FuelEntries");

            migrationBuilder.AddColumn<Guid>(
                name: "CarEntityId",
                table: "FuelEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FuelEntries_CarEntityId",
                table: "FuelEntries",
                column: "CarEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelEntries_Cars_CarEntityId",
                table: "FuelEntries",
                column: "CarEntityId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelEntries_Cars_CarEntityId",
                table: "FuelEntries");

            migrationBuilder.DropIndex(
                name: "IX_FuelEntries_CarEntityId",
                table: "FuelEntries");

            migrationBuilder.DropColumn(
                name: "CarEntityId",
                table: "FuelEntries");

            migrationBuilder.CreateIndex(
                name: "IX_FuelEntries_CarId",
                table: "FuelEntries",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelEntries_Cars_CarId",
                table: "FuelEntries",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
