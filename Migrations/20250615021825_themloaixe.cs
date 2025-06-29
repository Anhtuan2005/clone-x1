using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bike.Migrations
{
    /// <inheritdoc />
    public partial class themloaixe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiXe",
                table: "Xe");

            migrationBuilder.RenameColumn(
                name: "GiaThuê",
                table: "Xe",
                newName: "GiaThue");

            migrationBuilder.AddColumn<int>(
                name: "MaLoaiXe",
                table: "Xe",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LoaiXe",
                columns: table => new
                {
                    MaLoaiXe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiXe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiXe", x => x.MaLoaiXe);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Xe_MaLoaiXe",
                table: "Xe",
                column: "MaLoaiXe");

            migrationBuilder.AddForeignKey(
                name: "FK_Xe_LoaiXe_MaLoaiXe",
                table: "Xe",
                column: "MaLoaiXe",
                principalTable: "LoaiXe",
                principalColumn: "MaLoaiXe",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Xe_LoaiXe_MaLoaiXe",
                table: "Xe");

            migrationBuilder.DropTable(
                name: "LoaiXe");

            migrationBuilder.DropIndex(
                name: "IX_Xe_MaLoaiXe",
                table: "Xe");

            migrationBuilder.DropColumn(
                name: "MaLoaiXe",
                table: "Xe");

            migrationBuilder.RenameColumn(
                name: "GiaThue",
                table: "Xe",
                newName: "GiaThuê");

            migrationBuilder.AddColumn<string>(
                name: "LoaiXe",
                table: "Xe",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
