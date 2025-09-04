using Inventoria.Data;
using Inventoria.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventoria.Services
{
    public class SearchService
    {
        private readonly AppDbContext _db;
        public SearchService(AppDbContext db) { _db = db; }

        public async Task<(IEnumerable<Inventory>, IEnumerable<Item>)> SearchAsync(string query)
        {
            query = query.Trim();
            var inv = await _db.Inventories
                .Where(i => EF.Functions.ILike(i.Title, $"%{query}%") ||
                            EF.Functions.ILike(i.DescriptionMd ?? "", $"%{query}%"))
                .OrderByDescending(i => i.UpdatedAt)
                .Take(50)
                .ToListAsync();

            var items = await _db.Items.Include(i => i.Inventory)
                .Where(i => EF.Functions.ILike(i.CustomId, $"%{query}%"))
                .Take(100)
                .ToListAsync();

            return (inv, items);
        }
    }
}
