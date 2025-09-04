using Inventoria.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventoria.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index(string? q)
        {
            if (!string.IsNullOrWhiteSpace(q))
            {
                var inv = await _db.Inventories.Where(i => EF.Functions.ILike(i.Title, $"%{q}%")).ToListAsync();
                ViewBag.Query = q;
                return View(inv);
            }

            var latest = await _db.Inventories
                .Include(i => i.Owner)
                .OrderByDescending(i => i.CreatedAt).Take(10).ToListAsync();

            var popular = await _db.Inventories
                .OrderByDescending(i => _db.Items.Count(x => x.InventoryId == i.Id))
                .Take(5).ToListAsync();

            ViewBag.Popular = popular;
            return View(latest);
        }
    }
}
