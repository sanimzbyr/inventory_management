using System.ComponentModel.DataAnnotations;

namespace Inventoria.Models
{
    public class InventoryField
    {
        public int Id { get; set; }
        public Guid InventoryId { get; set; }
        public Inventory? Inventory { get; set; }
        public FieldType Type { get; set; }
        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool ShowInTable { get; set; } = true;
        public int DisplayOrder { get; set; }
        public string? ConstraintsJson { get; set; } // optional tuning
    }
}
