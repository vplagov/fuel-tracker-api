using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFuelEntryDateToDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"FuelEntries\" ALTER COLUMN \"Date\" TYPE date USING \"Date\"::date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "FuelEntries",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
