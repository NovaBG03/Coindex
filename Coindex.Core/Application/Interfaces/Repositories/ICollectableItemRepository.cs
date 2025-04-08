using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Filters;

namespace Coindex.Core.Application.Interfaces.Repositories;

public interface ICollectableItemRepository : IRepository<CollectableItem>
{
    Task<IEnumerable<CollectableItem>> GetPagedItemsAsync(int pageNumber, int pageSize,
        CollectableItemFilter? filter = null);

    Task<int> GetTotalCountAsync(CollectableItemFilter? filter = null);
    Task<CollectableItem?> GetItemByIdWithTagsAsync(int id);
}
