using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RingSoft.ChurchLogix.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class fiscalyear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FiscalYearEnd",
                table: "SystemPreferences",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FiscalYearStart",
                table: "SystemPreferences",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FiscalYearEnd",
                table: "SystemPreferences");

            migrationBuilder.DropColumn(
                name: "FiscalYearStart",
                table: "SystemPreferences");
        }
    }
}
