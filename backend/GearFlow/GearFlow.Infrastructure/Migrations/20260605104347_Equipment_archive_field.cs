using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GearFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Equipment_archive_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "Equipments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "Equipments");
        }
    }
}
