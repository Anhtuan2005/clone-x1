using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bike.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using bike.Repository;
using Microsoft.AspNetCore.Authorization;
using bike.Attributes;

namespace bike.Controllers
{
    [CustomAuthorize("Admin", "Staff")]
    public class XeController : Controller
    {
        private readonly BikeDbContext _context;

        // Constructor
        public XeController(BikeDbContext context)
        {
            _context = context;
        }

        // Helper method để kiểm tra xe có đang được thuê không
        private async Task<bool> IsCarRented(int maXe)
        {
            return await _context.HopDong
                .AnyAsync(h => h.MaXe == maXe && h.TrangThai == "Đang thuê");
        }

        // GET: Xe - Hiển thị danh sách xe
        public async Task<IActionResult> Index(string searchString, int? loaiXe, string hangXe, string trangThai)
        {
            // Query cơ bản
            var xeQuery = _context.Xe.Include(x => x.LoaiXe).AsQueryable();

            // Tìm kiếm theo tên xe
            if (!string.IsNullOrEmpty(searchString))
            {
                xeQuery = xeQuery.Where(x => x.TenXe.Contains(searchString));
            }

            // Lọc theo loại xe
            if (loaiXe.HasValue)
            {
                xeQuery = xeQuery.Where(x => x.MaLoaiXe == loaiXe.Value);
            }

            // Lọc theo hãng xe
            if (!string.IsNullOrEmpty(hangXe))
            {
                xeQuery = xeQuery.Where(x => x.HangXe == hangXe);
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(trangThai))
            {
                xeQuery = xeQuery.Where(x => x.TrangThai == trangThai);
            }

            // Thống kê
            ViewBag.TongSoXe = await _context.Xe.CountAsync();
            ViewBag.XeSanSang = await _context.Xe.CountAsync(x => x.TrangThai == "Sẵn sàng");
            ViewBag.DangChoThue = await _context.Xe.CountAsync(x => x.TrangThai == "Đang thuê");
            ViewBag.BaoTri = await _context.Xe.CountAsync(x => x.TrangThai == "Bảo trì");

            // Dropdown lists
            ViewBag.LoaiXeList = new SelectList(await _context.LoaiXe.ToListAsync(), "MaLoaiXe", "TenLoaiXe");
            ViewBag.HangXeList = new SelectList(await _context.Xe.Select(x => x.HangXe).Distinct().ToListAsync());
            ViewBag.TrangThaiList = new SelectList(new[] { "Sẵn sàng", "Đang thuê", "Bảo trì" });

            return View(await xeQuery.ToListAsync());
        }

        // GET: Xe/Create - Form thêm xe mới
        public async Task<IActionResult> Create()
        {
            ViewBag.MaLoaiXe = new SelectList(await _context.LoaiXe.ToListAsync(), "MaLoaiXe", "TenLoaiXe");
            ViewBag.TrangThaiList = new SelectList(new[] { "Sẵn sàng" });
            return View();
        }

        // POST: Xe/Create - Xử lý thêm xe mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Xe xe, IFormFile? hinhAnh)
        {
            // Kiểm tra biển số đã tồn tại chưa
            if (await _context.Xe.AnyAsync(x => x.BienSoXe == xe.BienSoXe))
            {
                ModelState.AddModelError("BienSoXe", "Biển số xe này đã tồn tại trong hệ thống!");
            }

            if (ModelState.IsValid)
            {
                // Xử lý upload hình ảnh
                if (hinhAnh != null && hinhAnh.Length > 0)
                {
                    // Tạo tên file duy nhất
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(hinhAnh.FileName);
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/xe", fileName);

                    // Lưu file
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await hinhAnh.CopyToAsync(stream);
                    }

                    xe.HinhAnhXe = fileName;
                }

                _context.Add(xe);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm xe mới thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLoaiXe = new SelectList(await _context.LoaiXe.ToListAsync(), "MaLoaiXe", "TenLoaiXe", xe.MaLoaiXe);
            ViewBag.TrangThaiList = new SelectList(new[] { "Sẵn sàng", "Đang thuê", "Bảo trì" }, xe.TrangThai);
            return View(xe);
        }

        // GET: Xe/Edit/5 - Form sửa xe
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xe = await _context.Xe.FindAsync(id);
            if (xe == null)
            {
                return NotFound();
            }

            // Kiểm tra xe có đang được thuê không
            if (await IsCarRented(xe.MaXe))
            {
                TempData["Error"] = "Không thể chỉnh sửa xe đang cho thuê!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLoaiXe = new SelectList(await _context.LoaiXe.ToListAsync(), "MaLoaiXe", "TenLoaiXe", xe.MaLoaiXe);
            ViewBag.TrangThaiList = new SelectList(new[] { "Sẵn sàng", "Đang thuê", "Bảo trì" }, xe.TrangThai);
            return View(xe);
        }

        // POST: Xe/Edit/5 - Xử lý cập nhật xe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Xe xe, IFormFile? hinhAnh)
        {
            if (id != xe.MaXe)
            {
                return NotFound();
            }

            // Kiểm tra xe có đang được thuê không
            if (await IsCarRented(xe.MaXe))
            {
                TempData["Error"] = "Không thể chỉnh sửa xe đang cho thuê!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra biển số đã tồn tại chưa (ngoại trừ xe hiện tại)
            if (await _context.Xe.AnyAsync(x => x.BienSoXe == xe.BienSoXe && x.MaXe != id))
            {
                ModelState.AddModelError("BienSoXe", "Biển số xe này đã được sử dụng cho xe khác!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin xe cũ để xử lý hình ảnh
                    var oldXe = await _context.Xe.AsNoTracking().FirstOrDefaultAsync(x => x.MaXe == id);

                    // Xử lý upload hình ảnh mới
                    if (hinhAnh != null && hinhAnh.Length > 0)
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(oldXe?.HinhAnhXe))
                        {
                            string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/xe", oldXe.HinhAnhXe);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        // Lưu ảnh mới
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(hinhAnh.FileName);
                        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/xe", fileName);

                        using (var stream = new FileStream(uploadPath, FileMode.Create))
                        {
                            await hinhAnh.CopyToAsync(stream);
                        }

                        xe.HinhAnhXe = fileName;
                    }
                    else
                    {
                        // Giữ nguyên hình ảnh cũ nếu không upload mới
                        xe.HinhAnhXe = oldXe?.HinhAnhXe;
                    }

                    _context.Update(xe);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin xe thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XeExists(xe.MaXe))
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

            ViewBag.MaLoaiXe = new SelectList(await _context.LoaiXe.ToListAsync(), "MaLoaiXe", "TenLoaiXe", xe.MaLoaiXe);
            ViewBag.TrangThaiList = new SelectList(new[] { "Sẵn sàng", "Đang thuê", "Bảo trì" }, xe.TrangThai);
            return View(xe);
        }

        // GET: Xe/Delete/5 - Xác nhận xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xe = await _context.Xe
                .Include(x => x.LoaiXe)
                .FirstOrDefaultAsync(m => m.MaXe == id);
            if (xe == null)
            {
                return NotFound();
            }

            // Kiểm tra xe có đang được thuê không
            if (await IsCarRented(xe.MaXe))
            {
                TempData["Error"] = "Không thể xóa xe đang cho thuê!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra xe có lịch sử hợp đồng không
            var hasContracts = await _context.HopDong.AnyAsync(h => h.MaXe == xe.MaXe);
            ViewBag.HasContracts = hasContracts;

            return View(xe);
        }

        // POST: Xe/Delete/5 - Xử lý xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var xe = await _context.Xe.FindAsync(id);
            if (xe == null)
            {
                return NotFound();
            }

            // Kiểm tra xe có đang được thuê không
            if (await IsCarRented(xe.MaXe))
            {
                TempData["Error"] = "Không thể xóa xe đang cho thuê!";
                return RedirectToAction(nameof(Index));
            }

            // Xóa hình ảnh nếu có
            if (!string.IsNullOrEmpty(xe.HinhAnhXe))
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/xe", xe.HinhAnhXe);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Xe.Remove(xe);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa xe thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Xe/Details/5 - Xem chi tiết xe
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xe = await _context.Xe
                .Include(x => x.LoaiXe)
                .FirstOrDefaultAsync(m => m.MaXe == id);

            if (xe == null)
            {
                return NotFound();
            }

            // Lấy lịch sử hợp đồng của xe
            ViewBag.LichSuHopDong = await _context.HopDong
                .Where(h => h.MaXe == xe.MaXe)
                .OrderByDescending(h => h.NgayTao)
                .Take(5)
                .ToListAsync();

            return View(xe);
        }

        // Kiểm tra xe có tồn tại không
        private bool XeExists(int id)
        {
            return _context.Xe.Any(e => e.MaXe == id);
        }

        // AJAX: Kiểm tra biển số có tồn tại không
        [HttpGet]
        public async Task<IActionResult> KiemTraBienSo(string bienSoXe, int? maXe)
        {
            if (string.IsNullOrEmpty(bienSoXe))
            {
                return Json(true);
            }

            // Nếu đang edit (có maXe), loại trừ xe hiện tại
            bool exists;
            if (maXe.HasValue)
            {
                exists = await _context.Xe.AnyAsync(x => x.BienSoXe == bienSoXe && x.MaXe != maXe.Value);
            }
            else
            {
                exists = await _context.Xe.AnyAsync(x => x.BienSoXe == bienSoXe);
            }

            if (exists)
            {
                return Json("Biển số xe này đã tồn tại!");
            }

            return Json(true);
        }
    }
}