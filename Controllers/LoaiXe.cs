using Microsoft.AspNetCore.Mvc;
using bike.Models;
using bike.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using bike.Attributes;
namespace bike.Controllers
{
    namespace bike.Controllers
    {
       [CustomAuthorize("Admin", "Staff")]
        public class LoaiXeController : Controller

        {
            private readonly BikeDbContext _context;
            public LoaiXeController(BikeDbContext context)
            {
                _context = context;
            }

            // GET: /LoaiXe
            public async Task<IActionResult> Index()
            {
                return View(await _context.LoaiXe.ToListAsync());
            }

            // GET: /LoaiXe/Create
            public IActionResult Create()
            {
                return View();
            }

            // POST: /LoaiXe/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("TenLoaiXe")] LoaiXe loaiXe)
            {
                if (ModelState.IsValid)
                {
                    loaiXe.NgayTao = DateTime.Now;
                    _context.Add(loaiXe);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(loaiXe);
            }

            // GET: /LoaiXe/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null) return NotFound();
                var loaiXe = await _context.LoaiXe.FindAsync(id);
                if (loaiXe == null) return NotFound();
                return View(loaiXe);
            }

            // POST: /LoaiXe/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("MaLoaiXe,TenLoaiXe,NgayTao,NgayCapNhat")] LoaiXe loaiXe)
            {
                if (id != loaiXe.MaLoaiXe) return NotFound();

                if (ModelState.IsValid)
                {
                    try
                    {
                        loaiXe.NgayCapNhat = DateTime.Now;
                        _context.Update(loaiXe);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LoaiXeExists(loaiXe.MaLoaiXe)) return NotFound();
                        else throw;
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(loaiXe);
            }

            // GET: /LoaiXe/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null) return NotFound();
                var loaiXe = await _context.LoaiXe.FindAsync(id);
                if (loaiXe == null) return NotFound();
                return View(loaiXe);
            }

            // POST: /LoaiXe/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var loaiXe = await _context.LoaiXe.FindAsync(id);
                if (loaiXe != null)
                {
                    _context.LoaiXe.Remove(loaiXe);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }

            private bool LoaiXeExists(int id)
            {
                return _context.LoaiXe.Any(e => e.MaLoaiXe == id);
            }
        }
    }
}
