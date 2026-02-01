using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Authentication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedstaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicenseNumber",
                table: "Users",
                newName: "StaffType");

            migrationBuilder.AddColumn<bool>(
                name: "IsStaff",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "StaffServiceId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStaff",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StaffServiceId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "StaffType",
                table: "Users",
                newName: "LicenseNumber");
        }
    }
}
