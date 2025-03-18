using Coindex.Core.Domain.Entities;

namespace Coindex.Core.Application.Interfaces.Services;

public interface ICollectableItemService
{
    Task<IEnumerable<CollectableItem>> GetPagedItemsAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<CollectableItem?> GetItemByIdAsync(int id);
}
