using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bike.Migrations
{
    /// <inheritdoc />
    public partial class addatcho : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatCho",
                columns: table => new
                {
                    MaDatCho = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaXe = table.Column<int>(type: "int", nullable: false),
                    MaUser = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayNhanXe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTraXe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatCho", x => x.MaDatCho);
                    table.ForeignKey(
                        name: "FK_DatCho_Users_MaUser",
                        column: x => x.MaUser,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DatCho_Xe_MaXe",
                        column: x => x.MaXe,
                        principalTable: "Xe",
                        principalColumn: "MaXe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatCho_MaUser",
                table: "DatCho",
                column: "MaUser");

            migrationBuilder.CreateIndex(
                name: "IX_DatCho_MaXe",
                table: "DatCho",
                column: "MaXe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatCho");
        }
    }
}
