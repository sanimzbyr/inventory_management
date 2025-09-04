using Inventoria.Data;
using Inventoria.Hubs;
using Inventoria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Inventoria.Controllers.Api
{
    [Route("api/discussion/{inventoryId}")]
    [ApiController]
    public class DiscussionController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<DiscussionHub> _hub;
        private readonly UserManager<ApplicationUser> _um;
        public DiscussionController(AppDbContext db, IHubContext<DiscussionHub> hub, UserManager<ApplicationUser> um)
        {
            _db = db; _hub = hub; _um = um;
        }

        [HttpGet]
        public async Task<IEnumerable<object>> Get(Guid inventoryId)
        {
            return await _db.DiscussionPosts.Where(p => p.InventoryId == inventoryId)
                .OrderBy(p => p.Id).Take(200)
                .Select(p => new { p.Id, p.ContentMd, p.CreatedAt, author = p.Author!.UserName })
                .ToListAsync();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(Guid inventoryId, [FromBody] PostText m)
        {
            var userId = _um.GetUserId(User)!;
            var post = new DiscussionPost { InventoryId = inventoryId, AuthorId = userId, ContentMd = m.Text, CreatedAt = DateTime.UtcNow };
            _db.DiscussionPosts.Add(post);
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("newPost", new { id = post.Id, post.ContentMd, post.CreatedAt, author = User.Identity!.Name });
            return Ok();
        }

        public class PostText { public string Text { get; set; } = ""; }
    }
}
