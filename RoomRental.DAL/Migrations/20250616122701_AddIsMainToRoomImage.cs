﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomRental.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsMainToRoomImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "RoomImages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "RoomImages");
        }
    }
}
