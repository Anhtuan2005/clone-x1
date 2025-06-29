using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bike.Models;
using bike.ViewModels;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using bike.Repository;

namespace bike.Controllers
{
    public class AccountController : Controller
    {
        private readonly BikeDbContext _context;

        public AccountController(BikeDbContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email đã tồn tại chưa
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được đăng ký!");
                    return View(model);
                }

                // Tạo user mới
                var user = new User
                {
                    Ten = model.Ten,
                    Email = model.Email,
                    MatKhau = HashPassword(model.MatKhau),
                    SoDienThoai = model.SoDienThoai,
                    VaiTro = "User" // Mặc định là User, có thể thay đổi nếu cần
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["ThongBao"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Tìm user theo email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && VerifyPassword(model.MatKhau, user.MatKhau))
                {
                    // Tạo claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Ten),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.VaiTro)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    // Đăng nhập
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Chuyển hướng theo role
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Redirect theo role mặc định
                    switch (user.VaiTro)
                    {
                        case "Admin":
                            return RedirectToAction("Index", "BaoCao");
                        case "Staff":
                            return RedirectToAction("Index", "QuanLyHopDong");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng!");
            }

            return View(model);
        }
        // GET: Account/Logout - Không cần token
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // POST: Account/LogoutConfirm - Cần token (đổi tên để tránh trùng)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Logout")] // Vẫn map về route /Account/Logout
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Helper methods
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            string hashOfInput = HashPassword(password);
            return hashOfInput == hashedPassword;
        }
    }
}
