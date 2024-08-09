using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RingSoft.ChurchLogix.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class MembersGivingFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberGiving_Members_MemberId",
                table: "MemberGiving");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberGivingDetails_Funds_FundId",
                table: "MemberGivingDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberGivingDetails_MemberGiving_MemberGivingId",
                table: "MemberGivingDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberGivingDetails",
                table: "MemberGivingDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberGiving",
                table: "MemberGiving");

            migrationBuilder.RenameTable(
                name: "MemberGivingDetails",
                newName: "MembersGivingDetails");

            migrationBuilder.RenameTable(
                name: "MemberGiving",
                newName: "MembersGiving");

            migrationBuilder.RenameIndex(
                name: "IX_MemberGivingDetails_FundId",
                table: "MembersGivingDetails",
                newName: "IX_MembersGivingDetails_FundId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberGiving_MemberId",
                table: "MembersGiving",
                newName: "IX_MembersGiving_MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MembersGivingDetails",
                table: "MembersGivingDetails",
                columns: new[] { "MemberGivingId", "RowId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MembersGiving",
                table: "MembersGiving",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MembersGiving_Members_MemberId",
                table: "MembersGiving",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MembersGivingDetails_Funds_FundId",
                table: "MembersGivingDetails",
                column: "FundId",
                principalTable: "Funds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MembersGivingDetails_MembersGiving_MemberGivingId",
                table: "MembersGivingDetails",
                column: "MemberGivingId",
                principalTable: "MembersGiving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembersGiving_Members_MemberId",
                table: "MembersGiving");

            migrationBuilder.DropForeignKey(
                name: "FK_MembersGivingDetails_Funds_FundId",
                table: "MembersGivingDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MembersGivingDetails_MembersGiving_MemberGivingId",
                table: "MembersGivingDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MembersGivingDetails",
                table: "MembersGivingDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MembersGiving",
                table: "MembersGiving");

            migrationBuilder.RenameTable(
                name: "MembersGivingDetails",
                newName: "MemberGivingDetails");

            migrationBuilder.RenameTable(
                name: "MembersGiving",
                newName: "MemberGiving");

            migrationBuilder.RenameIndex(
                name: "IX_MembersGivingDetails_FundId",
                table: "MemberGivingDetails",
                newName: "IX_MemberGivingDetails_FundId");

            migrationBuilder.RenameIndex(
                name: "IX_MembersGiving_MemberId",
                table: "MemberGiving",
                newName: "IX_MemberGiving_MemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberGivingDetails",
                table: "MemberGivingDetails",
                columns: new[] { "MemberGivingId", "RowId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberGiving",
                table: "MemberGiving",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberGiving_Members_MemberId",
                table: "MemberGiving",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberGivingDetails_Funds_FundId",
                table: "MemberGivingDetails",
                column: "FundId",
                principalTable: "Funds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberGivingDetails_MemberGiving_MemberGivingId",
                table: "MemberGivingDetails",
                column: "MemberGivingId",
                principalTable: "MemberGiving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
