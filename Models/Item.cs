using System.ComponentModel.DataAnnotations;

namespace Inventoria.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public Inventory? Inventory { get; set; }

        [Required, MaxLength(200)]
        public string CustomId { get; set; } = string.Empty;

        public string CreatedById { get; set; } = string.Empty;
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public byte[]? RowVersion { get; set; }

        public ICollection<ItemFieldValue> FieldValues { get; set; } = new List<ItemFieldValue>();
    }

    public class ItemFieldValue
    {
        public int Id { get; set; }
        public Guid ItemId { get; set; }
        public Item? Item { get; set; }
        public int FieldId { get; set; }
        public InventoryField? Field { get; set; }
        public string? Value { get; set; } // stores numbers, booleans, links, text as string
    }

    public class ItemLike
    {
        public Guid ItemId { get; set; }
        public Item? Item { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
