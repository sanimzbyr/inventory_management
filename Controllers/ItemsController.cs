using Inventoria.Data;
using Inventoria.Models;
using Inventoria.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventoria.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _um;
        private readonly CustomIdService _id;

        public ItemsController(AppDbContext db, UserManager<ApplicationUser> um, CustomIdService id)
        {
            _db = db; _um = um; _id = id;
        }

        [AllowAnonymous]
        public async Task<IActionResult> ByInventory(Guid inventoryId)
        {
            var inv = await _db.Inventories.Include(i => i.Fields).FirstOrDefaultAsync(i => i.Id == inventoryId);
            if (inv == null) return NotFound();
            var items = await _db.Items.Where(i => i.InventoryId == inventoryId).OrderByDescending(i => i.CreatedAt).ToListAsync();
            ViewBag.Inventory = inv;
            return View("Index", items);
        }

        public async Task<IActionResult> Create(Guid inventoryId)
        {
            var inv = await _db.Inventories.Include(i => i.Fields).FirstOrDefaultAsync(i => i.Id == inventoryId);
            if (inv == null) return NotFound();
            var m = new Item { InventoryId = inventoryId, CustomId = await _id.GenerateAsync(inventoryId) };
            return View("Edit", m);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }


        [HttpPost]
        public async Task<IActionResult> Save(Item m)
        {
            if (!ModelState.IsValid) return View("Edit", m);
            var userId = _um.GetUserId(User)!;
            if (m.Id == Guid.Empty)
            {
                m.Id = Guid.NewGuid();
                m.CreatedAt = m.UpdatedAt = DateTime.UtcNow;
                m.CreatedById = userId;
                _db.Items.Add(m);
            }
            else
            {
                var existing = await _db.Items.FindAsync(m.Id);
                if (existing == null) return NotFound();
                existing.CustomId = m.CustomId;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            try
            {
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(ByInventory), new { inventoryId = m.InventoryId });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Save failed (possible duplicate Custom ID). Please adjust and try again.");
                return View("Edit", m);
            }
        }
    }
}
