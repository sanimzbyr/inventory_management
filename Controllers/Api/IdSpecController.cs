using Inventoria.Data;
using Inventoria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventoria.Controllers.Api
{
    [Route("api/idspec/{inventoryId}")]
    [ApiController]
    [Authorize]
    public class IdSpecController : ControllerBase
    {
        private readonly AppDbContext _db;
        public IdSpecController(AppDbContext db) { _db = db; }

        [HttpPost]
        public async Task<IActionResult> Save(Guid inventoryId, [FromBody] List<IdElementDto> elements)
        {
            var spec = await _db.CustomIdSpecs.FirstOrDefaultAsync(x => x.InventoryId == inventoryId);
            if (spec == null) 
            { 
                spec = new CustomIdSpec { InventoryId = inventoryId }; 
                _db.CustomIdSpecs.Add(spec); 
            }

            spec.ElementsJson = System.Text.Json.JsonSerializer.Serialize(
                elements.Select(e => new IdElement(e.TypeEnum, e.Text, e.Format))
            );

            await _db.SaveChangesAsync();
            return Ok();
        }


        public class IdElementDto
        {
            public string Type { get; set; } = "Fixed";
            public string? Text { get; set; }
            public string? Format { get; set; }
            public IdElementType TypeEnum => Enum.TryParse<IdElementType>(Type, out var t) ? t : IdElementType.Fixed;
        }
    }
}
