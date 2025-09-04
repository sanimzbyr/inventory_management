using System.ComponentModel.DataAnnotations;

namespace Inventoria.Models
{
    public class DiscussionPost
    {
        public long Id { get; set; }
        public Guid InventoryId { get; set; }
        public Inventory? Inventory { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; }
        [Required]
        public string ContentMd { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
