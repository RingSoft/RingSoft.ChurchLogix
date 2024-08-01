using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RingSoft.ChurchLogix.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class FundPeriodTotals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundPeriodTotals",
                columns: table => new
                {
                    FundId = table.Column<int>(type: "integer", nullable: false),
                    PeriodType = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    TotalIncome = table.Column<double>(type: "numeric", nullable: false),
                    TotalExpenses = table.Column<double>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundPeriodTotals", x => new { x.FundId, x.PeriodType, x.Date });
                    table.ForeignKey(
                        name: "FK_FundPeriodTotals_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundPeriodTotals");
        }
    }
}
