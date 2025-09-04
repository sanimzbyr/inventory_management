using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Inventoria.Models
{
    public class CustomIdSpec
    {
        [Key]
        public Guid InventoryId { get; set; } // 1:1 relationship with Inventory
        public Inventory? Inventory { get; set; }

        // Ordered list of elements in JSON; each item has: type, text, format
        public string ElementsJson { get; set; } = "[]";

        // Example of the generated ID based on elements
        public string? Example { get; set; }

        // Helper property to deserialize ElementsJson into a list of IdElements
        [NotMapped]
        public List<IdElement> Elements
        {
            get => JsonSerializer.Deserialize<List<IdElement>>(ElementsJson) ?? new List<IdElement>();
            set => ElementsJson = JsonSerializer.Serialize(value);
        }

        // Method to update the Example field based on the current Elements
        public void GenerateExample()
        {
            Example = string.Join("-", Elements.Select(e => e.Type switch
            {
                IdElementType.Fixed => e.Text ?? "",
                IdElementType.RandomD6 => new Random().Next(100000, 999999).ToString(),
                IdElementType.RandomD9 => new Random().Next(100000000, 999999999).ToString(),
                IdElementType.Random20 => new Random().Next(0, 1048576).ToString("X"),
                IdElementType.Guid => Guid.NewGuid().ToString(),
                IdElementType.DateTime => DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                IdElementType.Sequence => "SEQ001", // Placeholder for sequence logic
                _ => ""
            }));
        }
    }

    public record IdElement(IdElementType Type, string? Text, string? Format);
}
