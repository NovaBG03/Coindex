using Coindex.Core.Domain.Enums;

namespace Coindex.Core.Domain.Filters;

public class CollectableItemFilter()
{
    public string? Name { get; set; }
    public int? TagId { get; set; }
    public ItemCondition? Condition { get; set; }
}
