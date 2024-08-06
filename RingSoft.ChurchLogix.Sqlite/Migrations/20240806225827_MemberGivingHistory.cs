using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RingSoft.ChurchLogix.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class MemberGivingHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MembersGivingHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    FundId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<double>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembersGivingHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembersGivingHistory_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MembersGivingHistory_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembersGivingHistory_FundId",
                table: "MembersGivingHistory",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_MembersGivingHistory_MemberId",
                table: "MembersGivingHistory",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembersGivingHistory");
        }
    }
}
