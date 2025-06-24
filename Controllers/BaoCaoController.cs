using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using bike.Repository;
using bike.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using bike.Attributes;

namespace bike.Controllers
{
    [CustomAuthorize("Admin", "Staff")]
    public class BaoCaoController : Controller
    {
        private readonly BikeDbContext _context;

        public BaoCaoController(BikeDbContext context)
        {
            _context = context;
        }

        // GET: BaoCao
        public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay)
        {
            // Nếu không có ngày, mặc định lấy 30 ngày gần nhất
            var endDate = denNgay ?? DateTime.Now.Date;
            var startDate = tuNgay ?? endDate.AddDays(-30);

            var viewModel = new BaoCaoViewModel
            {
                TuNgay = startDate,
                DenNgay = endDate
            };

            // 1. Thống kê tổng quan
            // Tổng đơn đặt xe trong khoảng thời gian
            viewModel.TongDonDatXe = await _context.DatCho
                .Where(d => d.NgayDat >= startDate && d.NgayDat <= endDate.AddDays(1))
                .CountAsync();

            // Đơn chờ xử lý
            viewModel.DonChoXuLy = await _context.DatCho
                .Where(d => d.TrangThai == "Chờ xác nhận" || d.TrangThai == "Đang giữ chỗ")
                .CountAsync();

            // Doanh thu hôm nay
            var today = DateTime.Now.Date;
            viewModel.DoanhThuHomNay = await _context.HopDong
                .Where(h => h.NgayTao.Date == today)
                .SumAsync(h => h.TongTien);

            // Xe đang cho thuê
            viewModel.XeDangChoThue = await _context.Xe
                .Where(x => x.TrangThai == "Đang thuê")
                .CountAsync();

            // 2. Tính % tăng/giảm so với kỳ trước
            var previousPeriodDays = (endDate - startDate).Days;
            var previousStartDate = startDate.AddDays(-previousPeriodDays - 1);
            var previousEndDate = startDate.AddDays(-1);

            var previousDonDat = await _context.DatCho
                .Where(d => d.NgayDat >= previousStartDate && d.NgayDat <= previousEndDate)
                .CountAsync();

            if (previousDonDat > 0)
            {
                viewModel.PhanTramDonDat = ((double)(viewModel.TongDonDatXe - previousDonDat) / previousDonDat) * 100;
            }

            // 3. Dữ liệu biểu đồ doanh thu (7 ngày gần nhất)
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.Date.AddDays(-6 + i))
                .ToList();

            foreach (var date in last7Days)
            {
                var doanhThuNgay = await _context.HopDong
                    .Where(h => h.NgayTao.Date == date)
                    .SumAsync(h => h.TongTien);

                viewModel.BieuDoDoanhThu.Add(new BieuDoItem
                {
                    Label = date.ToString("dd/MM"),
                    Value = doanhThuNgay
                });
            }

            // 4. Dữ liệu biểu đồ đơn đặt xe (7 ngày gần nhất)
            foreach (var date in last7Days)
            {
                var donDatNgay = await _context.DatCho
                    .Where(d => d.NgayDat.Date == date)
                    .CountAsync();

                viewModel.BieuDoDonDat.Add(new BieuDoItem
                {
                    Label = date.ToString("dd/MM"),
                    Value = donDatNgay
                });
            }

            // 5. Top 5 xe được thuê nhiều nhất
            var topXe = await _context.HopDong
                .Where(h => h.NgayTao >= startDate && h.NgayTao <= endDate.AddDays(1))
                .GroupBy(h => new { h.MaXe, h.Xe.TenXe, h.Xe.BienSoXe })
                .Select(g => new XeThueNhieuItem
                {
                    TenXe = g.Key.TenXe,
                    BienSo = g.Key.BienSoXe,
                    SoLanThue = g.Count(),
                    DoanhThu = g.Sum(h => h.TongTien)
                })
                .OrderByDescending(x => x.SoLanThue)
                .Take(5)
                .ToListAsync();

            viewModel.TopXeThueNhieu = topXe;

            // 6. 10 đơn đặt gần đây
            var donGanDay = await _context.DatCho
                .Include(d => d.Xe)
                .OrderByDescending(d => d.NgayDat)
                .Take(10)
                .Select(d => new DonDatGanDayItem
                {
                    MaDatCho = d.MaDatCho,
                    TenKhach = d.HoTen,
                    TenXe = d.Xe.TenXe,
                    NgayDat = d.NgayDat,
                    TrangThai = d.TrangThai,
                    TongTien = d.TongTienDuKien
                })
                .ToListAsync();

            viewModel.DonDatGanDay = donGanDay;

            return View(viewModel);
        }

        // GET: BaoCao/DoanhThuTheoThang - Báo cáo doanh thu theo tháng
        public async Task<IActionResult> DoanhThuTheoThang(int? year)
        {
            var currentYear = year ?? DateTime.Now.Year;
            var monthlyRevenue = new List<BieuDoItem>();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(currentYear, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var revenue = await _context.HopDong
                    .Where(h => h.NgayTao >= startDate && h.NgayTao <= endDate)
                    .SumAsync(h => h.TongTien);

                monthlyRevenue.Add(new BieuDoItem
                {
                    Label = $"Tháng {month}",
                    Value = revenue
                });
            }

            ViewBag.Year = currentYear;
            ViewBag.Years = Enumerable.Range(2020, DateTime.Now.Year - 2020 + 1).Reverse();

            return View(monthlyRevenue);
        }

        // GET: BaoCao/ExportExcel - Xuất báo cáo Excel
        public async Task<IActionResult> ExportExcel(DateTime? tuNgay, DateTime? denNgay)
        {
            // TODO: Implement Excel export using EPPlus or similar library
            TempData["Info"] = "Chức năng xuất Excel đang được phát triển";
            return RedirectToAction(nameof(Index));
        }
    }
}