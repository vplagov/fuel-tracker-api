using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelTracker.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExistingFuelEntriesToFullTank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"FuelEntries\" SET \"IsFullTank\" = true;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"FuelEntries\" SET \"IsFullTank\" = false;");
        }
    }
}
