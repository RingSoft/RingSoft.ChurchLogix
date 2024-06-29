using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RingSoft.ChurchLogix.MasterData.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Churches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "nvarchar", maxLength: 50, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar", maxLength: 250, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar", maxLength: 250, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Platform = table.Column<byte>(type: "smallint", nullable: false),
                    Server = table.Column<string>(type: "nvarchar", maxLength: 50, nullable: true),
                    Database = table.Column<string>(type: "nvarchar", maxLength: 50, nullable: true),
                    AuthenticationType = table.Column<byte>(type: "smallint", nullable: true),
                    Username = table.Column<string>(type: "nvarchar", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar", maxLength: 50, nullable: true),
                    DefaultUser = table.Column<int>(type: "integer", nullable: true),
                    MigrateDb = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Churches", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Churches");
        }
    }
}
