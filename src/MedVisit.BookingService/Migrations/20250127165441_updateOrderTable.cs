using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedVisit.BookingService.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServiceName",
                table: "Orders",
                newName: "TimeSlot");

            migrationBuilder.AddColumn<int>(
                name: "MedServiceId",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MedServiceName",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicalWorkerFullName",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MedicalWorkerId",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimeSlotId",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedServiceId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MedServiceName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MedicalWorkerFullName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MedicalWorkerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TimeSlotId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TimeSlot",
                table: "Orders",
                newName: "ServiceName");
        }
    }
}
