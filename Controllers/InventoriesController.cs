using System.Text.Json;
using Inventoria.Data;
using Inventoria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventoria.Controllers
{
    [Authorize]
    public class InventoriesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public InventoriesController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var invs = await _db.Inventories
                .Include(i => i.Owner)
                .OrderByDescending(i => i.UpdatedAt)
                .ToListAsync();
            return View(invs);
        }

        // GET: Create Inventory
        public IActionResult Create() => View(new Inventory());

        // POST: Create Inventory
        [HttpPost]
        public async Task<IActionResult> Create(Inventory m, string[] customIdElements)
        {
            if (!ModelState.IsValid) return View(m);

            // Set the owner ID
            m.OwnerId = _userManager.GetUserId(User)!;
            m.CreatedAt = m.UpdatedAt = DateTime.UtcNow;

            // Add inventory to the database
            _db.Inventories.Add(m);
            await _db.SaveChangesAsync();

            // If customIdElements are provided, create the CustomIdSpec and save it
            if (customIdElements != null && customIdElements.Any())
            {
                var idElements = customIdElements.Select(el =>
                {
                    var parts = el.Split("_");
                    var type = Enum.Parse<IdElementType>(parts[0]);
                    var value = parts.Length > 1 ? parts[1] : null;
                    return new IdElement(type, value, null);
                }).ToList();

                // Serialize the IdElement list to JSON
                var elementsJson = JsonSerializer.Serialize(idElements);

                // Create and save the CustomIdSpec
                var customIdSpec = new CustomIdSpec
                {
                    InventoryId = m.Id,
                    ElementsJson = elementsJson
                };

                _db.CustomIdSpecs.Add(customIdSpec);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Manage", new { id = m.Id });
        }



        [AllowAnonymous]
        public async Task<IActionResult> View(Guid id, string tab = "items")
        {
            var inv = await _db.Inventories.Include(i => i.Fields).FirstOrDefaultAsync(i => i.Id == id);
            if (inv == null) return NotFound();
            ViewBag.Tab = tab;
            if (tab == "items")
            {
                ViewBag.Items = await _db.Items
                    .Where(x => x.InventoryId == id)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            return View("Manage", inv);
        }

        public async Task<IActionResult> Manage(Guid id, string tab = "items")
        {
            var inv = await _db.Inventories.Include(i => i.Fields).FirstOrDefaultAsync(i => i.Id == id);
            if (inv == null) return NotFound();
            if (inv.OwnerId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
                return RedirectToAction(nameof(View), new { id, tab });
            ViewBag.Tab = tab;
            if (tab == "items")
            {
                ViewBag.Items = await _db.Items
                    .Where(x => x.InventoryId == id)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();
            }
            return View(inv);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGeneral(Guid id, Inventory input)
        {
            var inv = await _db.Inventories.FindAsync(id);
            if (inv == null) return NotFound();
            if (inv.OwnerId != _userManager.GetUserId(User) && !User.IsInRole("Admin")) return Forbid();

            inv.Title = input.Title;
            inv.DescriptionMd = input.DescriptionMd;
            inv.Category = input.Category;
            inv.IsPublic = input.IsPublic;
            inv.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return Ok(new { status = "saved" });
        }
    }
}
