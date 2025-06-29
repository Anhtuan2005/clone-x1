using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bike.Migrations
{
    /// <inheritdoc />
    public partial class LinkChiTieuToXe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "ChiTieu",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "MaXe",
                table: "ChiTieu",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTieu_MaXe",
                table: "ChiTieu",
                column: "MaXe");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTieu_Xe_MaXe",
                table: "ChiTieu",
                column: "MaXe",
                principalTable: "Xe",
                principalColumn: "MaXe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTieu_Xe_MaXe",
                table: "ChiTieu");

            migrationBuilder.DropIndex(
                name: "IX_ChiTieu_MaXe",
                table: "ChiTieu");

            migrationBuilder.DropColumn(
                name: "MaXe",
                table: "ChiTieu");

            migrationBuilder.AlterColumn<string>(
                name: "GhiChu",
                table: "ChiTieu",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
