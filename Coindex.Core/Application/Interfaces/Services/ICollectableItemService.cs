using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Filters;

namespace Coindex.Core.Application.Interfaces.Services;

public interface ICollectableItemService
{
    Task<IEnumerable<CollectableItem>> GetPagedItemsAsync(int pageNumber, int pageSize,
        CollectableItemFilter? filter = null);

    Task<int> GetTotalCountAsync(CollectableItemFilter? filter = null);
    Task<CollectableItem?> GetItemByIdAsync(int id);
    Task<CollectableItem?> GetItemByIdWithTagsAsync(int id);
    Task<int> AddItemAsync(CollectableItem item);
    Task UpdateItemAsync(CollectableItem item);
}
