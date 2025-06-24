using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bike.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Role_MaVaiTro",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Users_MaVaiTro",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MaVaiTro",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaVaiTro",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.MaVaiTro);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_MaVaiTro",
                table: "Users",
                column: "MaVaiTro");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Role_MaVaiTro",
                table: "Users",
                column: "MaVaiTro",
                principalTable: "Role",
                principalColumn: "MaVaiTro");
        }
    }
}
