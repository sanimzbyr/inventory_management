using Inventoria.Data;
using Inventoria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventoria.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _um;
        private readonly RoleManager<IdentityRole> _rm;

        public AdminController(AppDbContext db, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
        {
            _db = db; _um = um; _rm = rm;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _db.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddAdmin(string id)
        {
            var user = await _um.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _um.AddToRoleAsync(user, "Admin");
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var user = await _um.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _um.RemoveFromRoleAsync(user, "Admin");
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> Block(string id)
        {
            var user = await _um.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _um.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> Unblock(string id)
        {
            var user = await _um.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _um.SetLockoutEndDateAsync(user, null);
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _um.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _um.DeleteAsync(user);
            return RedirectToAction(nameof(Users));
        }
    }
}
