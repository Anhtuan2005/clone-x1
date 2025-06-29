using bike.Models; // Thêm dòng này để sử dụng Model ChiTieu
using bike.Repository; // Thêm dòng này để sử dụng BikeDbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; // Thêm dòng này để dùng các hàm như ToListAsync()

namespace bike.Controllers
{
    public class QuanLyChiTieuController : Controller
    {
        private readonly BikeDbContext _context; // Biến để chứa DbContext

        // Constructor để inject DbContext
        public QuanLyChiTieuController(BikeDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            DateTime homNay = DateTime.Today;

            // Tính tổng chi tiêu trong ngày hôm nay
            decimal tongChiHomNay = _context.ChiTieu
                                          .Where(c => c.NgayChi.Date == homNay)
                                          .Sum(c => c.SoTien);

            // Tính tổng chi tiêu trong tháng hiện tại
            decimal tongChiThangNay = _context.ChiTieu
                                            .Where(c => c.NgayChi.Year == homNay.Year && c.NgayChi.Month == homNay.Month)
                                            .Sum(c => c.SoTien);

            // Đưa các giá trị đã tính toán vào ViewData để View có thể sử dụng
            ViewData["TongChiHomNay"] = tongChiHomNay;
            ViewData["TongChiThangNay"] = tongChiThangNay;


            // --- PHẦN LẤY DANH SÁCH CHI TIÊU (Giữ nguyên như cũ) ---
            var danhSachChiTieu = await _context.ChiTieu.Include(c => c.Xe).ToListAsync();

            return View(danhSachChiTieu);
        }
        // Action này có nhiệm vụ hiển thị ra form để người dùng nhập liệu
        public IActionResult Create()
        {
            // Tạo một SelectList chứa danh sách các xe để hiển thị trong dropdown
            // "MaXe" là giá trị của option, "TenXe" là text hiển thị
            ViewData["MaXe"] = new SelectList(_context.Xe, "MaXe", "BienSoXe");
            return View();
        }

        // Action này sẽ được gọi khi người dùng nhấn nút "Lưu" trên form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoiDung,SoTien,NgayChi,GhiChu")] ChiTieu chiTieu)
        {
            // Đặt giá trị Id mặc định vì nó là identity, database sẽ tự tăng
            // và loại bỏ nó khỏi ModelState để không bị lỗi validation
            ModelState.Remove("Id");

            if (ModelState.IsValid)
            {
                _context.Add(chiTieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Chuyển hướng về trang danh sách
            }
            // Nếu dữ liệu không hợp lệ, hiển thị lại form để người dùng sửa lỗi
            return View(chiTieu);
        }
        public async Task<IActionResult> ChiTiet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTieu = await _context.ChiTieu
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chiTieu == null)
            {
                return NotFound();
            }

            return View(chiTieu);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTieu = await _context.ChiTieu.FindAsync(id);
            if (chiTieu == null)
            {
                return NotFound();
            }
            // Tương tự Create, nhưng thêm tham số thứ 4 để chọn sẵn xe đã được liên kết trước đó
            ViewData["MaXe"] = new SelectList(_context.Xe, "MaXe", "BienSoXe", chiTieu.MaXe);
            return View(chiTieu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NoiDung,SoTien,NgayChi,GhiChu,MaXe")] ChiTieu chiTieu)
        {
            if (id != chiTieu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chiTieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTieuExists(chiTieu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chiTieu);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTieu = await _context.ChiTieu
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chiTieu == null)
            {
                return NotFound();
            }

            return View(chiTieu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chiTieu = await _context.ChiTieu.FindAsync(id);
            if (chiTieu != null)
            {
                _context.ChiTieu.Remove(chiTieu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTieuExists(int id)
        {
            return _context.ChiTieu.Any(e => e.Id == id);
        }
    }
}