namespace Inventoria.Models
{
    public enum InventoryCategory { Equipment, Furniture, Book, Document, Other }
    public enum FieldType { SingleLine, MultiLine, Numeric, DocumentLink, Boolean }
    public enum IdElementType { Fixed, Random20, Random32, RandomD6, RandomD9, Guid, DateTime, Sequence }
}
