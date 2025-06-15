using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomRental.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Rooms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "SquareMeters",
                table: "Rooms",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "SquareMeters",
                table: "Rooms");
        }
    }
}
