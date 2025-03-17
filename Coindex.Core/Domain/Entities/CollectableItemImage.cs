namespace Coindex.Core.Domain.Entities;

public class CollectableItemImage : BaseEntity
{
    public byte[] ImageData { get; set; } = [];
    public string ContentType { get; set; } = "";
    public bool IsPrimary { get; set; }

    public int CollectableItemId { get; set; }
    public CollectableItem CollectableItem { get; set; } = null!;
}
