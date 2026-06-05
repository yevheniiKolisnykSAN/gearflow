using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GearFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class equipmentType_archived_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "EquipmentTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "EquipmentTypes");
        }
    }
}
