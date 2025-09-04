using System.Security.Cryptography;
using System.Text;
using Inventoria.Data;
using Inventoria.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventoria.Services
{
    public class CustomIdService
    {
        private readonly AppDbContext _db;
        public CustomIdService(AppDbContext db) { _db = db; }

        public async Task<string> GenerateAsync(Guid inventoryId)
        {
            var spec = await _db.CustomIdSpecs.FirstOrDefaultAsync(x => x.InventoryId == inventoryId);
            var elements = new List<IdElement>();
            if (spec != null && !string.IsNullOrWhiteSpace(spec.ElementsJson))
            {
                elements = System.Text.Json.JsonSerializer.Deserialize<List<IdElement>>(spec.ElementsJson) ?? new();
            }
            if (elements.Count == 0)
            {
                elements.Add(new IdElement(IdElementType.Fixed, "ITM-", null));
                elements.Add(new IdElement(IdElementType.RandomD6, null, "D6"));
            }

            var sb = new StringBuilder();
            foreach (var e in elements)
            {
                switch (e.Type)
                {
                    case IdElementType.Fixed:
                        sb.Append(e.Text ?? "");
                        break;
                    case IdElementType.Random20:
                        sb.Append(ToHex(RandomNumberGenerator.GetInt32(1 << 20), e.Format));
                        break;
                    case IdElementType.Random32:
                        sb.Append(ToHex(RandomNumberGenerator.GetInt32(int.MaxValue), e.Format));
                        break;
                    case IdElementType.RandomD6:
                        sb.Append(RandomDigits(6));
                        break;
                    case IdElementType.RandomD9:
                        sb.Append(RandomDigits(9));
                        break;
                    case IdElementType.Guid:
                        sb.Append(Guid.NewGuid().ToString("N"));
                        break;
                    case IdElementType.DateTime:
                        sb.Append(DateTime.UtcNow.ToString(e.Format ?? "yyyyMMdd"));
                        break;
                    case IdElementType.Sequence:
                        var next = await NextSequenceAsync(inventoryId);
                        if (!string.IsNullOrWhiteSpace(e.Format))
                            sb.Append(next.ToString(e.Format));
                        else sb.Append(next);
                        break;
                }
            }
            return sb.ToString();
        }

        private static string ToHex(int value, string? fmt)
        {
            var hex = value.ToString("X");
            if (fmt != null && fmt.StartsWith("X"))
            {
                if (int.TryParse(fmt[1..], out var width))
                    return hex.PadLeft(width, '0').Substring(Math.Max(0, hex.Length - width));
            }
            return hex;
        }

        private static string RandomDigits(int n)
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[n];
            rng.GetBytes(bytes);
            var chars = bytes.Select(b => (char)('0' + (b % 10)));
            return new string(chars.ToArray());
        }

        private async Task<int> NextSequenceAsync(Guid inventoryId)
        {
            // compute max from existing items (cheap enough with index and projection)
            var max = await _db.Items.Where(i => i.InventoryId == inventoryId)
                                     .Select(i => i.CustomId)
                                     .ToListAsync();

            // Find trailing number in IDs:
            int best = 0;
            foreach (var s in max)
            {
                int num = 0;
                for (int i = s.Length - 1; i >= 0; i--)
                {
                    if (!char.IsDigit(s[i])) break;
                    num = (int)Math.Pow(10, s.Length - 1 - i) * (s[i] - '0') + num;
                }
                if (num > best) best = num;
            }
            return best + 1;
        }
    }
}
