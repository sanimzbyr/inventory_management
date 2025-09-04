using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Inventoria.Models
{
    public class Inventory
    {
        public Guid Id { get; set; }

        public CustomIdSpec? CustomIdSpec { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? DescriptionMd { get; set; }
        
        public InventoryCategory Category { get; set; } = InventoryCategory.Other;y
        public bool IsPublic { get; set; } = false;

        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser? Owner { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public byte[]? RowVersion { get; set; }

        public ICollection<InventoryField> Fields { get; set; } = new List<InventoryField>();

        public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();

        public ICollection<InventoryAccess> AccessList { get; set; } = new List<InventoryAccess>();

        public Inventory()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public class InventoryTag
    {
        public Guid InventoryId { get; set; }
        public Inventory? Inventory { get; set; }
        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
    }

    public class InventoryAccess
    {
        public Guid InventoryId { get; set; }
        public Inventory? Inventory { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}
