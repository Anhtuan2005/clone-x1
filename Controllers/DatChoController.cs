using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bike.Repository;
using bike.Models;
using bike.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using bike.Attributes;

namespace bike.Controllers
{
    [Authorize]
    public class DatChoController : Controller
    {
        private readonly BikeDbContext _context;

        public DatChoController(BikeDbContext context)
        {
            _context = context;
        }

        // GET: DatCho/Create/5 - Form đặt giữ chỗ
        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin xe
            var xe = await _context.Xe
                .Include(x => x.LoaiXe)
                .FirstOrDefaultAsync(x => x.MaXe == id);

            if (xe == null || xe.TrangThai != "Sẵn sàng")
            {
                TempData["Error"] = "Xe không khả dụng để đặt!";
                return RedirectToAction("XemChiTiet", "Home", new { id });
            }

            // Tạo ViewModel
            var viewModel = new DatChoViewModel
            {
                MaXe = xe.MaXe,
                TenXe = xe.TenXe,
                BienSoXe = xe.BienSoXe,
                GiaThue = xe.GiaThue,
                HinhAnhXe = xe.HinhAnhXe
            };

            // Nếu đã đăng nhập, điền sẵn thông tin
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users.FindAsync(int.Parse(userId));
                if (user != null)
                {
                    viewModel.HoTen = user.Ten;
                    viewModel.Email = user.Email;
                    viewModel.SoDienThoai = user.SoDienThoai ?? "";
                }
            }

            return View(viewModel);
        }

        // POST: DatCho/Create - Xử lý đặt chỗ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DatChoViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xe còn sẵn sàng không
                var xe = await _context.Xe.FindAsync(model.MaXe);
                if (xe == null || xe.TrangThai != "Sẵn sàng")
                {
                    ModelState.AddModelError("", "Xe không còn khả dụng!");
                    return View(model);
                }

                // Tạo phiếu đặt chỗ
                var datCho = new DatCho
                {
                    MaXe = model.MaXe,
                    HoTen = model.HoTen,
                    SoDienThoai = model.SoDienThoai,
                    Email = model.Email,
                    NgayNhanXe = model.NgayNhanXe,
                    NgayTraXe = model.NgayTraXe,
                    GhiChu = model.GhiChu,
                    NgayDat = DateTime.Now,
                    TrangThai = "Chờ xác nhận"
                };

                // Nếu đã đăng nhập, lưu MaUser
                if (User.Identity.IsAuthenticated)
                {
                    datCho.MaUser = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                }

                _context.DatCho.Add(datCho);
                await _context.SaveChangesAsync();

                // Chuyển đến trang xác nhận
                return RedirectToAction("Confirmation", new { id = datCho.MaDatCho });
            }

            // Nếu có lỗi, load lại thông tin xe
            var xeInfo = await _context.Xe.FindAsync(model.MaXe);
            if (xeInfo != null)
            {
                model.TenXe = xeInfo.TenXe;
                model.BienSoXe = xeInfo.BienSoXe;
                model.GiaThue = xeInfo.GiaThue;
                model.HinhAnhXe = xeInfo.HinhAnhXe;
            }

            return View(model);
        }

        // GET: DatCho/Confirmation/5 - Trang xác nhận đặt chỗ thành công
        public async Task<IActionResult> Confirmation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datCho = await _context.DatCho
                .Include(d => d.Xe)
                .ThenInclude(x => x.LoaiXe)
                .FirstOrDefaultAsync(d => d.MaDatCho == id);

            if (datCho == null)
            {
                return NotFound();
            }

            return View(datCho);
        }

        // GET: DatCho/MyReservations - Xem danh sách đặt chỗ của tôi (nếu đã đăng nhập)
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> MyReservations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var reservations = await _context.DatCho
                .Include(d => d.Xe)
                .Where(d => d.MaUser == userId)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            return View(reservations);
        }
        // GET: DatCho/CheckEmail - Kiểm tra email đã tồn tại
        [HttpGet]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { exists = false });
            }

            // Kiểm tra trong bảng User
            var userExists = await _context.Users.AnyAsync(u => u.Email == email);

            // Hoặc kiểm tra trong lịch sử đặt chỗ
            var hasBookingHistory = await _context.DatCho.AnyAsync(d => d.Email == email);

            return Json(new
            {
                exists = userExists || hasBookingHistory,
                isRegistered = userExists
            });
        }
    }
}