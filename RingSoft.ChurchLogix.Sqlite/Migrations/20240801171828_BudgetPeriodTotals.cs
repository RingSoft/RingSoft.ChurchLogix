using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RingSoft.ChurchLogix.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class BudgetPeriodTotals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetsPeriodTotals",
                columns: table => new
                {
                    BudgetId = table.Column<int>(type: "integer", nullable: false),
                    PeriodType = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Total = table.Column<double>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetsPeriodTotals", x => new { x.BudgetId, x.PeriodType, x.Date });
                    table.ForeignKey(
                        name: "FK_BudgetsPeriodTotals_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetsPeriodTotals");
        }
    }
}
