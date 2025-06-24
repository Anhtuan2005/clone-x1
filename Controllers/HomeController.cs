using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bike.Repository;
using bike.Models;
using bike.ViewModel;
using System.Diagnostics;

namespace bike.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BikeDbContext _context;

        public HomeController(ILogger<HomeController> logger, BikeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            // Tạo ViewModel
            var viewModel = new XeMayLoaiXe();

            // Lấy danh sách loại xe
            viewModel.DanhSachLoaiXe = await _context.LoaiXe
                .OrderBy(l => l.TenLoaiXe)
                .ToListAsync();

            // Lấy danh sách xe (có thể lọc theo xe thuê nhiều nhất)
            viewModel.DanhSachXeMay = await _context.Xe
                .Include(x => x.LoaiXe)
                .OrderByDescending(x => x.MaXe) // Hoặc order theo số lần thuê nếu có
                .Take(8) // Lấy 8 xe đầu
                .ToListAsync();

            return View(viewModel);
        }
        // GET: Home/XemChiTiet/5
        public async Task<IActionResult> XemChiTiet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin xe kèm loại xe
            var xe = await _context.Xe
                .Include(x => x.LoaiXe)
                .FirstOrDefaultAsync(x => x.MaXe == id);

            if (xe == null)
            {
                return NotFound();
            }

            // Lấy xe liên quan (cùng loại)
            ViewBag.XeLienQuan = await _context.Xe
                .Include(x => x.LoaiXe)
                .Where(x => x.MaLoaiXe == xe.MaLoaiXe && x.MaXe != xe.MaXe)
                .Take(4)
                .ToListAsync();

            return View(xe);
        }
    }
}