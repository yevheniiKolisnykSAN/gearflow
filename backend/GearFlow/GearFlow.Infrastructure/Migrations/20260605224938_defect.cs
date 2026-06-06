using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GearFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class defect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservationId",
                table: "Defects",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Defects_ReservationId",
                table: "Defects",
                column: "ReservationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Defects_Reservations_ReservationId",
                table: "Defects",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Defects_Reservations_ReservationId",
                table: "Defects");

            migrationBuilder.DropIndex(
                name: "IX_Defects_ReservationId",
                table: "Defects");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "Defects");
        }
    }
}
