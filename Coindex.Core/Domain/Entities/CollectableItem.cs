namespace Coindex.Core.Domain.Entities;

public abstract class CollectableItem : BaseEntity
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime AcquiredDate { get; set; } = DateTime.UtcNow;
    public DateTime? SoldDate { get; set; }

    public decimal PurchasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? CurrentMarketValue { get; set; }

    public List<Tag> Tags { get; set; } = [];

    public List<CollectableItemImage> Images { get; set; } = [];
}
