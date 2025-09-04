using Inventoria.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventoria.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<InventoryField> InventoryFields => Set<InventoryField>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<ItemFieldValue> ItemFieldValues => Set<ItemFieldValue>();
        public DbSet<InventoryAccess> InventoryAccesses => Set<InventoryAccess>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<InventoryTag> InventoryTags => Set<InventoryTag>();
        public DbSet<DiscussionPost> DiscussionPosts => Set<DiscussionPost>();
        public DbSet<ItemLike> ItemLikes => Set<ItemLike>();
        public DbSet<CustomIdSpec> CustomIdSpecs => Set<CustomIdSpec>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // existing keys/indexes...
            b.Entity<InventoryTag>().HasKey(x => new { x.InventoryId, x.TagId });
            b.Entity<InventoryAccess>().HasKey(x => new { x.InventoryId, x.UserId });
            b.Entity<ItemLike>().HasKey(x => new { x.ItemId, x.UserId });
            b.Entity<Item>().HasIndex(i => new { i.InventoryId, i.CustomId }).IsUnique();

            b.Entity<Inventory>().Property(i => i.RowVersion).IsRowVersion();
            b.Entity<Item>().Property(i => i.RowVersion).IsRowVersion();

            // 🔧 Make the 1:1 explicit: PK of CustomIdSpec is also its FK to Inventory.Id
            b.Entity<CustomIdSpec>()
                .HasKey(s => s.InventoryId);

            b.Entity<CustomIdSpec>()
                .HasOne(s => s.Inventory)
                .WithOne(i => i.CustomIdSpec) // if you didn’t add the back‑nav, use .WithOne()
                .HasForeignKey<CustomIdSpec>(s => s.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasPostgresExtension("pg_trgm");
            b.HasPostgresExtension("unaccent");
        }

    }
}
