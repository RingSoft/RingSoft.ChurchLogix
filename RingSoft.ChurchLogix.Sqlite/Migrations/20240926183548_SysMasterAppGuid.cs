using Microsoft.EntityFrameworkCore.Migrations;
using RingSoft.ChurchLogix.DataAccess;

#nullable disable

namespace RingSoft.ChurchLogix.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class SysMasterAppGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppGuid",
                table: "SystemMaster",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            DataAccessGlobals.MigrateAppGuid(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppGuid",
                table: "SystemMaster");
        }
    }
}
