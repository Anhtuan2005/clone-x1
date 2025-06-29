using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bike.Migrations
{
    /// <inheritdoc />
    public partial class editrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrangThai",
                table: "Users",
                newName: "VaiTro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VaiTro",
                table: "Users",
                newName: "TrangThai");
        }
    }
}
