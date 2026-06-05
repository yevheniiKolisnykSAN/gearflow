using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GearFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class typlocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "archived",
                table: "Locations",
                newName: "Archived");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Archived",
                table: "Locations",
                newName: "archived");
        }
    }
}
