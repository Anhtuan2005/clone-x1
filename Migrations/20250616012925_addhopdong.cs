using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bike.Migrations
{
    /// <inheritdoc />
    public partial class addhopdong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HopDong",
                columns: table => new
                {
                    MaHopDong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatCho = table.Column<int>(type: "int", nullable: true),
                    MaXe = table.Column<int>(type: "int", nullable: false),
                    HoTenKhach = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoCCCD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayNhanXe = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTraXeDuKien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTraXeThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GiaThueNgay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TienCoc = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhuPhi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNguoiTao = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopDong", x => x.MaHopDong);
                    table.ForeignKey(
                        name: "FK_HopDong_DatCho_MaDatCho",
                        column: x => x.MaDatCho,
                        principalTable: "DatCho",
                        principalColumn: "MaDatCho");
                    table.ForeignKey(
                        name: "FK_HopDong_Users_MaNguoiTao",
                        column: x => x.MaNguoiTao,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HopDong_Xe_MaXe",
                        column: x => x.MaXe,
                        principalTable: "Xe",
                        principalColumn: "MaXe",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaDatCho",
                table: "HopDong",
                column: "MaDatCho");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaNguoiTao",
                table: "HopDong",
                column: "MaNguoiTao");

            migrationBuilder.CreateIndex(
                name: "IX_HopDong_MaXe",
                table: "HopDong",
                column: "MaXe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HopDong");
        }
    }
}
