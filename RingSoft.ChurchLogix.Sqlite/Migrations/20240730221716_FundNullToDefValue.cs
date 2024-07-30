using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RingSoft.ChurchLogix.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class FundNullToDefValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TotalSpent",
                table: "Funds",
                type: "numeric",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TotalCollected",
                table: "Funds",
                type: "numeric",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Goal",
                table: "Funds",
                type: "numeric",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "numeric",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TotalSpent",
                table: "Funds",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "numeric");

            migrationBuilder.AlterColumn<double>(
                name: "TotalCollected",
                table: "Funds",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "numeric");

            migrationBuilder.AlterColumn<double>(
                name: "Goal",
                table: "Funds",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "numeric");
        }
    }
}
