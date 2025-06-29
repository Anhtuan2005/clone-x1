using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bike.Models;
using bike.Repository;
using bike.Attributes;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bike.Controllers
{
    [CustomAuthorize("Admin")] // Chỉ Admin mới được quản lý user
    public class QuanLyUser : Controller
    {
        private readonly BikeDbContext _context;

        public QuanLyUser(BikeDbContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.OrderBy(u => u.Ten).ToListAsync();
            return View(users);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(new[]
            {
                new { Value = "Admin", Text = "Quản trị viên" },
                new { Value = "Staff", Text = "Nhân viên" },
                new { Value = "User", Text = "Khách hàng" }
            }, "Value", "Text");

            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại!");
            }

            if (ModelState.IsValid)
            {
                // Hash password
                user.MatKhau = HashPassword(user.MatKhau);

                _context.Add(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm người dùng thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(new[]
            {
                new { Value = "Admin", Text = "Quản trị viên" },
                new { Value = "Staff", Text = "Nhân viên" },
                new { Value = "User", Text = "Khách hàng" }
            }, "Value", "Text", user.VaiTro);

            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = new SelectList(new[]
            {
                new { Value = "Admin", Text = "Quản trị viên" },
                new { Value = "Staff", Text = "Nhân viên" },
                new { Value = "User", Text = "Khách hàng" }
            }, "Value", "Text", user.VaiTro);

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, string? newPassword)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            // Check email unique
            if (await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != id))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.FindAsync(id);

                    // Update fields
                    existingUser.Ten = user.Ten;
                    existingUser.Email = user.Email;
                    existingUser.SoDienThoai = user.SoDienThoai;
                    existingUser.DiaChi = user.DiaChi;
                    existingUser.VaiTro = user.VaiTro;

                    // Update password if provided
                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        existingUser.MatKhau = HashPassword(newPassword);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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

            ViewBag.Roles = new SelectList(new[]
            {
                new { Value = "Admin", Text = "Quản trị viên" },
                new { Value = "Staff", Text = "Nhân viên" },
                new { Value = "User", Text = "Khách hàng" }
            }, "Value", "Text", user.VaiTro);

            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            // Không cho xóa chính mình
            if (user.Id == int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                TempData["Error"] = "Không thể xóa tài khoản của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);

            // Không cho xóa chính mình
            if (user.Id == int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value))
            {
                TempData["Error"] = "Không thể xóa tài khoản của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa người dùng thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper methods
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}