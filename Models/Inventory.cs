using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Inventoria.Models
{
    public class Inventory
    {
        public Guid Id { get; set; }

        // Add navigation to CustomIdSpec
        public CustomIdSpec? CustomIdSpec { get; set; }

        // Required fields for inventory creation
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? DescriptionMd { get; set; }
        
        // Category should be required (e.g., InventoryCategory.Equipment)
        public InventoryCategory Category { get; set; } = InventoryCategory.Other;

        // Public/Private access for the inventory
        public bool IsPublic { get; set; } = false;

        // Owner related fields
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser? Owner { get; set; }

        // Image URL for the inventory (optional)
        public string? ImageUrl { get; set; }

        // Timestamps for creation and updates
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // For optimistic concurrency control
        public byte[]? RowVersion { get; set; }

        // Associated fields for this inventory
        public ICollection<InventoryField> Fields { get; set; } = new List<InventoryField>();

        // Tags associated with this inventory
        public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();

        // Access list for user permissions on this inventory
        public ICollection<InventoryAccess> AccessList { get; set; } = new List<InventoryAccess>();

        // Constructor
        public Inventory()
        {
            // Ensure default values
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    // Associated models for Tags and InventoryAccess
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
