namespace Coindex.Core.Domain.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Color { get; set; } = "#808080";

    public List<CollectableItem> CollectableItems { get; set; } = [];
}
