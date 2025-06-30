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
        // GET: BaoCao/Index
        public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay)
        {
            // --- PHẦN 1: XỬ LÝ VÀ CHUẨN BỊ NGÀY THÁNG ---

            // Nếu người dùng không chọn ngày, mặc định sẽ là từ đầu tháng hiện tại đến ngày hôm nay
            var today = DateTime.Today;
            var startDate = tuNgay ?? new DateTime(today.Year, today.Month, 1);
            var endDate = denNgay ?? today;

            // Để đảm bảo các truy vấn bao gồm cả ngày cuối cùng, ta sẽ lấy đến cuối ngày (23:59:59)
            var fullEndDate = endDate.Date.AddDays(1).AddTicks(-1);

            // --- PHẦN 2: TRUY VẤN VÀ TÍNH TOÁN DỮ LIỆU ---

            // 2.1. Tính toán các chỉ số tài chính chính trong khoảng thời gian đã chọn
            decimal tongDoanhThu = await _context.HopDong
                .Where(h => h.TrangThai == "Hoàn thành" && h.NgayTraXeThucTe >= startDate && h.NgayTraXeThucTe <= fullEndDate)
                .SumAsync(h => (decimal?)h.TongTien) ?? 0;

            decimal tongChiPhi = await _context.ChiTieu
                .Where(c => c.NgayChi >= startDate && c.NgayChi <= fullEndDate)
                .SumAsync(c => (decimal?)c.SoTien) ?? 0;

            // 2.2. Tính toán các chỉ số hoạt động trong khoảng thời gian đã chọn (lấy từ code cũ của bạn)
            int tongDonDatXe = await _context.DatCho
                .Where(d => d.NgayDat >= startDate && d.NgayDat <= fullEndDate)
                .CountAsync();

            // 2.3. Tính toán các chỉ số tức thời (không phụ thuộc vào bộ lọc ngày)
            int donChoXuLy = await _context.DatCho
                .Where(d => d.TrangThai == "Chờ xác nhận" || d.TrangThai == "Đang giữ chỗ")
                .CountAsync();

            decimal doanhThuHomNay = await _context.HopDong
                .Where(h => h.TrangThai == "Hoàn thành" && h.NgayTraXeThucTe.HasValue && h.NgayTraXeThucTe.Value.Date == today)
                .SumAsync(h => (decimal?)h.TongTien) ?? 0;

            int xeDangChoThue = await _context.Xe
                .Where(x => x.TrangThai == "Đang thuê")
                .CountAsync();

            // 2.4. Lấy dữ liệu cho các bảng và biểu đồ (kết hợp từ code cũ của bạn)
            var topXe = await _context.HopDong
                .Where(h => h.NgayTao >= startDate && h.NgayTao <= fullEndDate)
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

            // --- PHẦN 3: TẠO VIEWMODEL VÀ GỬI DỮ LIỆU SANG VIEW ---

            var viewModel = new BaoCaoViewModel
            {
                // Gán ngày tháng cho bộ lọc
                TuNgay = startDate,
                DenNgay = endDate,

                // Gán dữ liệu tài chính
                TongDoanhThu = tongDoanhThu,
                TongChiPhi = tongChiPhi,
                LoiNhuan = tongDoanhThu - tongChiPhi,

                // Gán dữ liệu hoạt động
                TongDonDatXe = tongDonDatXe,
                DonChoXuLy = donChoXuLy,
                DoanhThuHomNay = doanhThuHomNay,
                XeDangChoThue = xeDangChoThue,

                // Gán dữ liệu bảng
                TopXeThueNhieu = topXe
                // Bạn có thể thêm các phần dữ liệu cho biểu đồ ở đây nếu cần
            };

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