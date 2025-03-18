using Coindex.Core.Domain.Enums;

namespace Coindex.Core.Domain.Entities;

public abstract class CollectableItem : BaseEntity
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Year { get; set; }
    public string Country { get; set; } = "";
    public decimal FaceValue { get; set; }
    public ItemCondition Condition { get; set; }

    public List<Tag> Tags { get; set; } = [];
}
